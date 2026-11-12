# 1. IMPORTS (Always at the absolute top)
# Python built-in System-specific parameters and functions module
import sys
# Python built-in library for BSD socket API
import socket
# Python built-in module for date and time 
import datetime

# 2. CONSTRAINTS / CONFIGURATION (Upper_snake_case)
APP_NAME = "Python Syntax Gym"
PORT = 5000
IS_RUNNING = True


# 3. HELPER FUNCTIONS / CLASSES
# Prints the prompt (for example: "You (Client): ")
# reads the input text
# returns text without white spaces from left or right
def prompt(text: str) -> str:
    return input(text).strip()

# Encodes text to UTF-8
def to_bytes(text: str) -> bytes:
    return text.encode("utf-8")

# Decodes raw bytes from UTF-8 to text String
def from_bytes(data: bytes) -> str:
    return data.decode("utf-8")

# Prints number of bytes of message
# then prints the echoed message
def echo_round(message: str) -> None:
    byte_string = to_bytes(message)
    count = len(byte_string)
    timestamp = datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")
    echoed_string = from_bytes(byte_string)
    print(f"[{timestamp}] byte count = {count} bytes | Echo: {echoed_string}")


# 4. MAIN EXECUTION ENTRY POINT
def main():
    # Print server banner
    print(f"App: {APP_NAME} | Port: {PORT} | Running: {IS_RUNNING}")

    # Main logic goes here...
    while True:
        # ask for message
        message = prompt(">Input your message here: ")
        # quitting chat
        if message == "/quit": 
            print("Closing ...")
            break
        # empty message
        if not message:
            print("Input a valid message!")
        echo_round(message)


if __name__ == "__main__":
    main()
