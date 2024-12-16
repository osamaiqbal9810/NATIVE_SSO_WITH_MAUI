package com.ps19.mykotlinlibrary

import android.app.Activity
import android.content.Context
import android.net.Uri
import android.util.Base64
import android.util.Log
import android.widget.Toast

import androidx.annotation.RequiresApi
import androidx.credentials.CredentialManager
import androidx.credentials.GetCredentialRequest
import androidx.credentials.GetCredentialResponse
import androidx.credentials.exceptions.GetCredentialCancellationException
import androidx.credentials.exceptions.GetCredentialException
import com.google.android.libraries.identity.googleid.GetGoogleIdOption
import com.google.android.libraries.identity.googleid.GoogleIdTokenCredential
import com.microsoft.identity.client.AcquireTokenParameters
import com.microsoft.identity.client.AuthenticationCallback
import com.microsoft.identity.client.IAuthenticationResult
import com.microsoft.identity.client.IPublicClientApplication
import com.microsoft.identity.client.ISingleAccountPublicClientApplication
import com.microsoft.identity.client.PublicClientApplication
import com.microsoft.identity.client.SingleAccountPublicClientApplication

import com.microsoft.identity.client.exception.MsalException
import kotlinx.coroutines.*
import org.json.JSONObject

interface Callback {
    fun invoke(result: GoogleSignInResult?)
}

data class GoogleSignInResult(
    var idToken: String?,
    var displayName: String?,
    var phoneNumber: String?,
    var familyName: String?,
    var givenName: String?,
    var profilePictureUri: Uri?,
    var credentialType: String?,
    var email: String?
)

class LoginHelper(private val context: Context) {

    private val credentialManager = CredentialManager.create(context)
    // MIcrosoft login
    private  lateinit var authClient: ISingleAccountPublicClientApplication
//    private var account: IAccount? = null
//    private lateinit var msalApp: PublicClientApplication
    private val  scopes = "User.Read"

    fun handleLoginGmailAsync(callback: Callback, clientId: String) {
        CoroutineScope(Dispatchers.IO).launch {
            try {
                val googleIdOption: GetGoogleIdOption = GetGoogleIdOption.Builder()
                    .setFilterByAuthorizedAccounts(false)
                    .setServerClientId(clientId)
                    .setAutoSelectEnabled(false)
                    .setNonce("Nonce")
                    .build()

                val request: GetCredentialRequest = GetCredentialRequest.Builder()
                    .addCredentialOption(googleIdOption)
                    .build()

                val result = credentialManager.getCredential(request = request, context = context)
                val userInfo = handleSignIn(result)

                withContext(Dispatchers.Main) {
                    val resultMessage = if (userInfo != null) {
                        "Success: Retrieved credential for ${userInfo.displayName}"
                    } else {
                        "Failed to retrieve credential"
                    }
                    callback.invoke(userInfo)
                }
            } catch (e: GetCredentialCancellationException) {
                Log.d("TAG", "Sign-in was canceled by the user")
                withContext(Dispatchers.Main) {
                    callback.invoke(null)
                }
            } catch (e: GetCredentialException) {
                Log.e("TAG", "Error during sign-in: ${e}")
                withContext(Dispatchers.Main) {
                    callback.invoke(null)
                }
            } catch (e: Exception) {
                Log.e("LoginHelper", "Login failed: ${e.message}")
                withContext(Dispatchers.Main) {
                    callback.invoke(null)
                }
            }
        }
    }


  fun handleLoginMicrosoftAsync(callback: Callback, clientId: String) {
      Log.e("handleLoginMicrosoft", "Starting Microsoft login")

   //   SingleAccountPublicClientApplication.create(context, R.raw.auth_config_ciam_auth);
      CoroutineScope(Dispatchers.Default).launch {
          try {
              Log.e("handleLoginMicrosoft", "Coroutine started on Default dispatcher")

              if (context !is Activity) {
                  Log.e("handleLoginMicrosoft", "Context is not an Activity")
                  withContext(Dispatchers.Main) {
                      Toast.makeText(context, "Context should be an Activity", Toast.LENGTH_LONG).show()
                  }
                  return@launch
              }

              Log.e("handleLoginMicrosoft", "Context is an Activity")
              withContext(Dispatchers.Main) {
                  Log.e("handleLoginMicrosoft", "Creating PublicClientApplication")
                  try {
                  PublicClientApplication.createSingleAccountPublicClientApplication(
                      context,
                      R.raw.auth_config_ciam_auth,
                      object : IPublicClientApplication.ISingleAccountApplicationCreatedListener {
                          override fun onCreated(application: ISingleAccountPublicClientApplication) {
                              Log.e("handleLoginMicrosoft", "PublicClientApplication created")
                              authClient = application

                              val activity = context as Activity
                              activity.runOnUiThread {
                                  try {
                                      Toast.makeText(context, "Starting authentication", Toast.LENGTH_LONG).show()
                                      val scopesList = listOf("User.Read")
                                      val parameters = AcquireTokenParameters.Builder()
                                          .startAuthorizationFromActivity(activity)
                                          .withScopes(scopesList)
                                          .withCallback(object : AuthenticationCallback {
                                              override fun onSuccess(authenticationResult: IAuthenticationResult) {
                                                  Log.d("handleLoginMicrosoft", "Authentication successful")
                                                  Log.d("MSAL", "Access Token: ${authenticationResult.accessToken}")
                                                //  callback.invoke(authenticationResult.accessToken)
                                              }

                                              override fun onError(exception: MsalException) {
                                                  Log.e("handleLoginMicrosoft", "Authentication error: $exception")
                                                  callback.invoke(null)
                                              }

                                              override fun onCancel() {
                                                  Log.d("handleLoginMicrosoft", "Authentication canceled by user")
                                                  callback.invoke(null)
                                              }
                                          })
                                          .build()

                                      authClient.acquireToken(parameters)
                                  } catch (e: Exception) {
                                      Log.e("handleLoginMicrosoft", "Error during authentication setup: ${e.message}")
                                      Toast.makeText(context, "Error: ${e.message}", Toast.LENGTH_LONG).show()
                                      callback.invoke(null)
                                  }
                              }
                          }

                          override fun onError(exception: MsalException?) {
                              Log.e("handleLoginMicrosoft", "Error creating PublicClientApplication: ${exception?.message}")
                              (context as Activity).runOnUiThread {
                                  Toast.makeText(
                                      context,
                                      "Error: ${exception?.message}",
                                      Toast.LENGTH_LONG
                                  ).show()
                              }
                              callback.invoke(null)
                          }
                      }
                  )
              } catch (e: Exception) {
                  Log.e("handleLoginMicrosoft", "Unhandled exception: ${e.message}")
                  withContext(Dispatchers.Main) {
                      Toast.makeText(context, "Login failed: ${e.message}", Toast.LENGTH_LONG).show()
                      callback.invoke(null)
                  }
              }
              }
          } catch (e: Exception) {
              Log.e("handleLoginMicrosoft", "Unhandled exception: ${e.message}")
              withContext(Dispatchers.Main) {
                  Toast.makeText(context, "Login failed: ${e.message}", Toast.LENGTH_LONG).show()
                  callback.invoke(null)
              }
          }
      }
  }


    private fun getEmailFromIdToken(idToken: String?): String? {
        if (idToken.isNullOrEmpty()) {
            return null
        }

        try {
            val parts = idToken.split(".")
            if (parts.size == 3) {
                val payload = String(Base64.decode(parts[1], Base64.URL_SAFE))
                val jsonObject = JSONObject(payload)
                return jsonObject.optString("email") // Extract email from the payload
            }
        } catch (e: Exception) {

            Log.e("TAG", "Error decoding ID Token", e)
        }
        return null
    }

    private fun handleSignIn(result: GetCredentialResponse): GoogleSignInResult? {
        Log.i("LoginHelper", "Login successful!")
        val credential = result.credential

        return if (credential.type == GoogleIdTokenCredential.TYPE_GOOGLE_ID_TOKEN_CREDENTIAL) {
            try {
                val googleIdTokenCredential = GoogleIdTokenCredential.createFrom(credential.data)
                val email = getEmailFromIdToken(googleIdTokenCredential.idToken)
                GoogleSignInResult(
                    idToken = googleIdTokenCredential.idToken,
                    displayName = googleIdTokenCredential.displayName,
                    phoneNumber = googleIdTokenCredential.phoneNumber,
                    familyName = googleIdTokenCredential.familyName,
                    givenName = googleIdTokenCredential.givenName,
                    profilePictureUri = googleIdTokenCredential.profilePictureUri,
                    credentialType = googleIdTokenCredential.type,
                    email = email
                )
            } catch (e: Exception) {
                Log.e("TAG", "Error processing Google ID Token", e)
                null
            }
        } else {
            Log.e("TAG", "Unexpected custom credential type")
            null
        }
    }


    companion object {
        @JvmStatic
        fun invokeHandleLoginGmail(context: Context, clientId: String, callback: Callback) {
            val helper = LoginHelper(context)
            Log.i("ClientId", clientId)
            helper.handleLoginGmailAsync(callback, clientId)
        }

        @JvmStatic
        fun invokeHandleLoginMicrosoft(context: Context, clientId: String, callback: Callback) {
            val helper = LoginHelper(context)
            Log.i("ClientId", clientId)

            helper.handleLoginMicrosoftAsync(callback, clientId)
        }
    }
}
