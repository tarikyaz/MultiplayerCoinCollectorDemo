const express = require('express');
const mongoose = require('mongoose');
const bcrypt = require('bcrypt');
const bodyParser = require('body-parser');
const session = require('express-session');
const jwt = require('jsonwebtoken');

 function InitAuth(){

    console.log("Called");
    const app = express();

    // Connect to MongoDB
    mongoose.connect('mongodb://localhost:27017/nodejs_login_signup', {
        useNewUrlParser: true,
        useUnifiedTopology: true,
    });

    mongoose.connection.on('error', console.error.bind(console, 'MongoDB connection error:'));
    mongoose.connection.once('open', () => console.log('Connected to MongoDB'));

    // Set up middleware
    app.use(bodyParser.urlencoded({ extended: true }));
    app.use(session({ secret: 'your-secret-key', resave: true, saveUninitialized: true }));

    // Define the User schema
    const userSchema = new mongoose.Schema({
        username: String,
        password: String,
    });

    // Create the User model
    const User = mongoose.model('User', userSchema);

    // Routes

    // Signup route
    app.post('/signup', async (req, res) => {
        const { username, password } = req.body;

        try {
            // Check if the user already exists
            const existingUser = await User.findOne({ username });
            if (existingUser) {
                res.status(409).send('Username already exists');
                return;
            }

            // Hash the password
            const hashedPassword = await bcrypt.hash(password, 10);

            // Create a new user
            const newUser = new User({
                username,
                password: hashedPassword,
            });

            // Save the user to the database
            await newUser.save();

            res.send('Signup successful! You can now login.');
        } catch (error) {
            console.error(error);
            res.status(500).send('Error saving user to database');
        }
    });

    // Login route
    app.post('/login', async (req, res) => {
        const { username, password } = req.body;

        // Find the user in the database
        const user = await User.findOne({ username });

        // Check if the user exists
        if (!user) {
            res.status(401).send('Invalid username or password');
            return;
        }

        // Compare the hashed password
        const passwordMatch = await bcrypt.compare(password, user.password);

        if (passwordMatch) {
            req.session.user = username;
            // Get the current date and time
            const currentDate = new Date();

            // Format the date as a string (e.g., "2023-01-25 15:30:00")
            const formattedDateString = `${currentDate.getFullYear()}-${(currentDate.getMonth() + 1).toString().padStart(2, '0')}-${currentDate.getDate().toString().padStart(2, '0')} ${currentDate.getHours().toString().padStart(2, '0')}:${currentDate.getMinutes().toString().padStart(2, '0')}:${currentDate.getSeconds().toString().padStart(2, '0')}`;

            console.log("Current Date and Time:", formattedDateString);
            var jwtSecret = formattedDateString + username;
            jwtSecret = shuffleString(jwtSecret);
            const tKey = jwt.sign({ username: user.username }, jwtSecret, { expiresIn: '1h' });
            res.json({ message: ' Login successful!', token: tKey });

        } else {
            res.status(401).send('Invalid username or password');
        }
    });

    // Logout route
    app.get('/logout', (req, res) => {
        req.session.destroy();
        res.send('Logged out successfully');
    });

    // Start the server
    const PORT = process.env.PORT || 3000;
    app.listen(PORT, () => console.log(`Server running on http://localhost:${PORT}`));
}
function shuffleString(inputString) {
    // Convert the string to an array of characters
    let charArray = inputString.split('');

    // Shuffle the array
    for (let i = charArray.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [charArray[i], charArray[j]] = [charArray[j], charArray[i]];
    }

    // Join the characters back into a string
    const shuffledString = charArray.join('');

    return shuffledString;
}
module.exports = { InitAuth }; // Export the Init function
