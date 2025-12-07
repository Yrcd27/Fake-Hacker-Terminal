# 🖥️ Fake Hacker Terminal CTF

A Matrix-style interactive terminal with 5 progressive Capture The Flag (CTF) challenges. Built for fun with ASP.NET Core and vanilla JavaScript.

![Terminal Preview](https://img.shields.io/badge/Status-Complete-success)
![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![License](https://img.shields.io/badge/License-MIT-blue)

## 🎯 Features

- **Matrix Rain Animation** - Authentic hacker aesthetic with falling code
- **5 Progressive Challenges** - Each level unlocks the next
- **Real-time Terminal Interface** - Type commands like a real terminal
- **Flag Format Guide** - Clear instructions for beginners
- **Completion Screen** - Celebrate your victory with style

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 SDK or later
- Any modern web browser

### Installation

```bash
# Clone the repository
git clone https://github.com/yourusername/FakeHackerTerminal.git

# Navigate to project directory
cd FakeHackerTerminal

# Run the application
dotnet run
```

Open your browser and navigate to `http://localhost:5000`

## 🎮 How to Play

### Flag Format
All flags follow this format: `FAKE{CONTENT_HERE}`
- Start with `FAKE{`
- End with `}`
- Content is UPPERCASE with underscores

### Commands
- `ls` - List files in current directory
- `cat <file>` - Read file contents
- `grep <pattern> <file>` - Search for patterns in files
- `submit <flag>` - Submit your discovered flag
- `help` - Get hints for current level
- `level` - Check your current progress
- `clear` - Clear the terminal screen



## 📁 Project Structure

```
FakeHackerTerminal/
├── Program.cs              # Backend API and CTF logic
├── wwwroot/
│   ├── index.html         # Main HTML structure
│   ├── script.js          # Terminal functionality
│   └── style.css          # Matrix theme styling
└── README.md              # This file
```

## 🛠️ Technology Stack

- **Backend**: ASP.NET Core 8.0 (Minimal API)
- **Frontend**: Vanilla JavaScript, HTML5, CSS3
- **Styling**: Custom Matrix-themed CSS with animations
- **Architecture**: Single-page application with REST API



## 📝 License

MIT License - Feel free to use this project for learning and fun!

## 🎓 Learning Outcomes

By completing this CTF, you'll practice:
- File system navigation
- Encoding/decoding techniques (Base64, ROT13)
- Cryptographic hash identification (MD5)
- Binary operations (XOR)
- Pattern matching and log analysis (grep)



## 👤 Author

Created for fun and learning. Enjoy hacking! 🚀

## 🙏 Acknowledgments

- Inspired by classic CTF challenges
- Matrix movie for the aesthetic
- The cybersecurity community

---

**Stay curious, keep hacking, and remember: The best hackers never stop learning.** 💚
