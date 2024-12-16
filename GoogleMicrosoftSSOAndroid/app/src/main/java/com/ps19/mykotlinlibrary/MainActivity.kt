package com.ps19.mykotlinlibrary

import android.os.Bundle
import android.view.ViewGroup
import android.widget.Button
import android.widget.LinearLayout
import android.widget.Toast
import androidx.activity.enableEdgeToEdge
import androidx.appcompat.app.AppCompatActivity

class MainActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
       // enableEdgeToEdge()

        // Create a LinearLayout programmatically
        val layout = LinearLayout(this)
        layout.orientation = LinearLayout.VERTICAL  // Set orientation to vertical
        layout.layoutParams = ViewGroup.LayoutParams(
            ViewGroup.LayoutParams.MATCH_PARENT,
            ViewGroup.LayoutParams.MATCH_PARENT
        )

        // Create the Button programmatically
        val btnGogoleLogin = Button(this)
        btnGogoleLogin.text = "Login with Google"
        btnGogoleLogin.layoutParams = LinearLayout.LayoutParams(
            LinearLayout.LayoutParams.WRAP_CONTENT,
            LinearLayout.LayoutParams.WRAP_CONTENT
        )

        // Set up the button click listener
        btnGogoleLogin.setOnClickListener {
            val clientId = "your-client-id"  // Replace with your actual client ID
            LoginHelper.invokeHandleLoginGmail(this, clientId, object : Callback {
                override fun invoke(result: GoogleSignInResult?) {
                    if (result != null) {
                        Toast.makeText(this@MainActivity, "Login successful!", Toast.LENGTH_SHORT).show()
                    } else {
                        Toast.makeText(this@MainActivity, "Login failed", Toast.LENGTH_SHORT).show()
                    }
                }
            })
        }

        // Create the Button programmatically
        val btnMicrosoftLogin = Button(this)
        btnMicrosoftLogin.text = "Login with Microsoft"
        btnMicrosoftLogin.layoutParams = LinearLayout.LayoutParams(
            LinearLayout.LayoutParams.WRAP_CONTENT,
            LinearLayout.LayoutParams.WRAP_CONTENT
        )

        // Set up the button click listener
        btnMicrosoftLogin.setOnClickListener {
            val clientId = "your-client-id"  // Replace with your actual client ID
            LoginHelper.invokeHandleLoginMicrosoft(this, clientId, object : Callback {
                override fun invoke(result: GoogleSignInResult?) {
                    if (result != null) {
                        Toast.makeText(this@MainActivity, "Login successful!", Toast.LENGTH_SHORT).show()
                    } else {
                        Toast.makeText(this@MainActivity, "Login failed", Toast.LENGTH_SHORT).show()
                    }
                }
            })
        }

        layout.addView(btnGogoleLogin)
        // Add the button to the layout
        layout.addView(btnMicrosoftLogin)

        // Set the newly created layout as the content view
        setContentView(layout)
    }
}
