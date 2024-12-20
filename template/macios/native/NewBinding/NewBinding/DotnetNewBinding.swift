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
import MSAL
@objc(DotnetNewBinding)
public class SSOHelper : NSObject
{

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
    
    @objc
    public static func performMicrosoftSignIn(withPresentingViewController presentingViewController: UIViewController, completion: @escaping (NSDictionary?, Error?) -> Void) {
        let signInManager = MicrosoftSignInManager()
        print("performMicrosoftSignIn")
        signInManager.signIn(presentingViewController: presentingViewController) { user, error in
            if let error = error {
                print("Error")
                completion(nil, error)
            } else if let user = user {
                print("User")
                completion(user, nil)
            } else {
                print("SignInError")
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

@objc public class MicrosoftSignInManager: NSObject {
    @objc public static let shared = MicrosoftSignInManager()

    @objc public func signIn(presentingViewController: UIViewController, completion: @escaping (_ user: NSDictionary?, _ error: Error?) -> Void) {
        do {
            // Configure the MSAL application
            //TODO: // handle clientid and other parameters from outside
            let authority = try MSALAADAuthority(url: URL(string: "https://login.microsoftonline.com/organizations")!)
            let config = MSALPublicClientApplicationConfig(clientId: "0058a1e6-3475-4ddd-8716-7b94aecd21f9", redirectUri: "msauth.com.ps19.chatwithdocsmobileapp://auth", authority: authority)
            
            let application = try MSALPublicClientApplication(configuration: config)
            
            // Configure interactive token acquisition
            let scopes = ["User.Read"]
            let webviewParameters = MSALWebviewParameters(authPresentationViewController: presentingViewController)
            let interactiveParameters = MSALInteractiveTokenParameters(scopes: scopes, webviewParameters: webviewParameters)
            
            application.acquireToken(with: interactiveParameters) { (result, error) in
                if let error = error {
                    print("Error acquiring token: \(error)")
                    completion(nil, error)
                    return
                }
                guard let result = result else {
                    let unknownError = NSError(domain: "SignInError", code: -1, userInfo: [NSLocalizedDescriptionKey: "Unknown error occurred"])
                    completion(nil, unknownError)
                    return
                }
                
                // Extract user details
                let userDetails: NSDictionary = [
                    "accessToken": result.accessToken,
                    "accountId": result.account.identifier ?? "",
                    "fullName": result.account.accountClaims?["name"] ?? "",
                    "email": result.account.username ?? "",
                    "scopes": result.scopes.joined(separator: ", ")
                ]
                print("Creation Success", result.account)
                completion(userDetails, nil)
            }
        } catch {
            print("Error creating MSAL application: \(error)")
            let configError = NSError(domain: "SignInError", code: -1, userInfo: [NSLocalizedDescriptionKey: "Unable to create application. Please check your MSAL configuration."])
            completion(nil, configError)
        }
    }
}
