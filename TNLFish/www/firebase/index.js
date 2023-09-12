// Import the functions you need from the SDKs you need
import { initializeApp } from "firebase/app";
import { getAnalytics } from "firebase/analytics";
// TODO: Add SDKs for Firebase products that you want to use
// https://firebase.google.com/docs/web/setup#available-libraries

// Your web app's Firebase configuration
// For Firebase JS SDK v7.20.0 and later, measurementId is optional
const firebaseConfig = {
    apiKey: "AIzaSyDrCSJ_pDl_LIzYyNVG8uhD6-IQJ9pAIGE",
    authDomain: "tnlfish-shop.firebaseapp.com",
    projectId: "tnlfish-shop",
    storageBucket: "tnlfish-shop.appspot.com",
    messagingSenderId: "299168830485",
    appId: "1:299168830485:web:401ae8dd17346497c92a1c",
    measurementId: "G-CK8KY2LZW5"
};

// Initialize Firebase
const app = initializeApp(firebaseConfig);
const analytics = getAnalytics(app);