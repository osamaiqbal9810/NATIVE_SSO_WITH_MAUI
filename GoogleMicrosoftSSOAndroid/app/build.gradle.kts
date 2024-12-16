plugins {
    //id("com.android.library")
    id("com.android.application")
    alias(libs.plugins.kotlin.android)
}

android {

    namespace = "com.ps19.mykotlinlibrary"
    compileSdk = 35

    defaultConfig {
        applicationId = "com.ps19.mykotlinlibrary"
      // testOptions.targetSdk = 34
        minSdk = 24
        targetSdk =34
        version = "1.0"

        testInstrumentationRunner = "androidx.test.runner.AndroidJUnitRunner"
    }

    buildTypes {
        release {
            isMinifyEnabled = false
            proguardFiles(
                getDefaultProguardFile("proguard-android-optimize.txt"),
                "proguard-rules.pro"
            )
        }
    }
    compileOptions {
        sourceCompatibility = JavaVersion.VERSION_11
        targetCompatibility = JavaVersion.VERSION_11
    }
    kotlinOptions {
        jvmTarget = "11"
    }
}

dependencies {

    implementation(libs.androidx.core.ktx)
    implementation(libs.androidx.appcompat)
    implementation(libs.material)
    implementation(libs.androidx.activity)
    implementation(libs.androidx.constraintlayout)
    testImplementation(libs.junit)
    androidTestImplementation(libs.androidx.junit)
    androidTestImplementation(libs.androidx.espresso.core)
    implementation("com.google.android.gms:play-services-auth:21.2.0")
    implementation("androidx.credentials:credentials:1.2.2")
    implementation("androidx.credentials:credentials-play-services-auth:1.2.2")
    implementation("com.google.android.libraries.identity.googleid:googleid:1.0.0")
    implementation("com.android.volley:volley:1.2.1")
    implementation("com.microsoft.identity.client:msal:5.+")
}
