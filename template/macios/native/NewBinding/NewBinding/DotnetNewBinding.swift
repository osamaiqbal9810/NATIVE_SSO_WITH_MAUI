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
                    "name": result.account.username ?? "",
                    "email": self.extractEmail(from: result.accessToken) ?? "",
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
    
    @objc public func extractEmail(from accessToken: String) -> String? {
        // Split the JWT into its components
        let tokenParts = accessToken.split(separator: ".")
        guard tokenParts.count == 3 else {
            print("Invalid token format")
            return nil
        }

        // Decode the payload (second part of the token)
        let payload = String(tokenParts[1])
        guard let payloadData = Data(base64Encoded: payload.fixBase64Padding(), options: .ignoreUnknownCharacters),
              let json = try? JSONSerialization.jsonObject(with: payloadData, options: []),
              let payloadDict = json as? [String: Any] else {
            print("Unable to decode token payload")
            return nil
        }

        // Extract the email
        if let email = payloadDict["preferred_username"] as? String {
            return email
        } else if let email = payloadDict["email"] as? String {
            return email
        } else {
            print("Email not found in token payload")
            return nil
        }
    }

}
// Helper function to fix Base64 padding if needed
extension String {
    func fixBase64Padding() -> String {
        let paddedLength = (self.count + 3) / 4 * 4
        return self.padding(toLength: paddedLength, withPad: "=", startingAt: 0)
    }
}

