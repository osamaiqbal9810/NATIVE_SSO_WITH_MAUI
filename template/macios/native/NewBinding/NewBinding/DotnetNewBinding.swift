//
//  DotnetNewBinding.swift
//  NewBinding
//
//  Created by .NET MAUI team on 6/18/24.
//

import SwiftUI
import GoogleSignIn
import GoogleSignInSwift
import UIKit

@objc(DotnetNewBinding)
public class DotnetNewBinding : NSObject
{

    @objc
    public static func getString(myString: String) -> String {
        return myString  + " from swift!"
    }

    @objc
    public static func performGoogleSignIn(withPresentingViewController presentingViewController: UIViewController, completion: @escaping (NSDictionary?, Error?) -> Void) {
        let signInManager = GoogleSignInManager()
        
        signInManager.signIn(presentingViewController: presentingViewController) { user, error in
            if let error = error {
                completion(nil, error)
            } else if let user = user {
                completion(user, nil)
            } else {
                completion(nil, NSError(domain: "SignInError", code: -1, userInfo: [NSLocalizedDescriptionKey: "Unknown error occurred"]))
            }
        }
    }
}

@objc public class GoogleSignInManager: NSObject {
    @objc public static let shared = GoogleSignInManager()

    @objc public func signIn(presentingViewController: UIViewController, completion: @escaping (_ user: NSDictionary?, _ error: Error?) -> Void) {
        GIDSignIn.sharedInstance.signIn(
            withPresenting: presentingViewController
        ) { result, error in
            if let error = error {
                completion(nil, error)
                return
            }
            guard let user = result?.user else {
                completion(nil, NSError(domain: "SignInError", code: -1, userInfo: [NSLocalizedDescriptionKey: "Failed to retrieve user details"]))
                return
            }
            
            // Create a dictionary with user details
            let userDetails: NSDictionary = [
                "idToken": user.idToken?.tokenString ?? "",
                "email": user.profile?.email ?? "",
                "fullName": user.profile?.name ?? "",
                "givenName": user.profile?.givenName ?? "",
                "familyName": user.profile?.familyName ?? "",
                "profilePicture": user.profile?.imageURL(withDimension: 100)?.absoluteString ?? ""
            ]
            
            completion(userDetails, nil)
        }
    }
}

