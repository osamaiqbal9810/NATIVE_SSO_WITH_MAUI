import SwiftUI
import UIKit

struct SSOLoginView: View {
    @State private var userInfo: [String: Any]? = nil
    @State private var errorMessage: String? = nil
    @State private var isLoading: Bool = false

    var body: some View {
        NavigationView {
            VStack(spacing: 20) {
                if let userInfo = userInfo {
                    VStack(alignment: .leading, spacing: 10) {
                        Text("User Info:").font(.headline)
                        ForEach(userInfo.keys.sorted(), id: \..self) { key in
                            if let value = userInfo[key] {
                                Text("\(key): \(value)")
                            }
                        }
                    }
                }

                if let errorMessage = errorMessage {
                    Text(errorMessage)
                        .foregroundColor(.red)
                }

                if isLoading {
                    ProgressView("Loading...")
                }

                Button(action: signInWithGoogle) {
                    Text("Sign in with Google")
                        .frame(maxWidth: .infinity)
                        .padding()
                        .background(Color.blue)
                        .foregroundColor(.white)
                        .cornerRadius(8)
                }
                .disabled(isLoading)

                Button(action: signInWithMicrosoft) {
                    Text("Sign in with Microsoft")
                        .frame(maxWidth: .infinity)
                        .padding()
                        .background(Color.green)
                        .foregroundColor(.white)
                        .cornerRadius(8)
                }
                .disabled(isLoading)
            }
            .padding()
            .navigationTitle("SSO Login")
        }
    }

    private func signInWithGoogle() {
        guard let presentingViewController = getRootViewController() else {
            errorMessage = "Unable to find root view controller."
            return
        }

        isLoading = true
        errorMessage = nil

        SSOHelper.performGoogleSignIn(withPresentingViewController: presentingViewController) { user, error in
            DispatchQueue.main.async {
                isLoading = false
                if let error = error {
                    errorMessage = "Google Sign-In Error: \(error.localizedDescription)"
                } else if let user = user as? [String: Any] {
                    userInfo = user
                } else {
                    errorMessage = "Unknown error occurred during Google Sign-In."
                }
            }
        }
    }

    private func signInWithMicrosoft() {
        guard let presentingViewController = getRootViewController() else {
            errorMessage = "Unable to find root view controller."
            return
        }

        isLoading = true
        errorMessage = nil

        SSOHelper.performMicrosoftSignIn(withPresentingViewController: presentingViewController) { user, error in
            DispatchQueue.main.async {
                isLoading = false
                if let error = error {
                    errorMessage = "Microsoft Sign-In Error: \(error.localizedDescription)"
                } else if let user = user as? [String: Any] {
                    userInfo = user
                } else {
                    errorMessage = "Unknown error occurred during Microsoft Sign-In."
                }
            }
        }
    }

    private func getRootViewController() -> UIViewController? {
        guard let windowScene = UIApplication.shared.connectedScenes.first as? UIWindowScene,
              let rootViewController = windowScene.windows.first?.rootViewController else {
            return nil
        }
        return rootViewController
    }
}

struct SSOLoginView_Previews: PreviewProvider {
    static var previews: some View {
        SSOLoginView()
    }
}
