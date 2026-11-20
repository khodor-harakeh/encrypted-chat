# ====================================================================================================================
# Imports
# ====================================================================================================================
# cryptography -> root third-party package for cryptographic operations
# hazmat -> hazardous materials houses low-level cryptographic building blocks
# primitives -> fundamental cryptographic algorithms
# ciphers -> algorithms designed specifically for encrypting/decrypting data
# aead -> Authenticated Encryption with Associated Data (confidentality + integrity)
# AESGCM -> Advanced Encryption Standard algorithm operating in Galois/Counter Mode
from cryptography.hazmat.primitives.ciphers.aead import AESGCM
# import InvalidTag for try...except tamper test
from cryptography.exceptions import InvalidTag
# Python built-in os module to interact directly with underlying operating system
import os
# Python built-in system module gives access to variables/functions that interact directly with the Python interpreter
import sys


# ====================================================================================================================
# Hardcoded Shared Key (for initial test)
# ====================================================================================================================
# 32 bytes of static key (represented as raw byte string)
SHARED_KEY = b"12345678901234567890123456789012" # Exactly 32 bytes

# ====================================================================================================================
# Helper Functions
# ====================================================================================================================
# Encryption
def encrypt_message(plaintext: str, key: bytes) -> tuple[bytes, bytes]:
    plaintext_bytes = plaintext.encode("utf-8")
    nonce = os.urandom(12)
    aesgcm = AESGCM(key)
    # encrypted payload
    ciphertext_and_tag = aesgcm.encrypt(nonce, plaintext_bytes, associated_data=None) 
    return (nonce, ciphertext_and_tag)

# Decryption
def decrypt_message(nonce: bytes, ciphertext_and_tag: bytes, key: bytes) -> str:
    aesgcm = AESGCM(key)
    decrypted_bytes = aesgcm.decrypt(nonce, ciphertext_and_tag, associated_data=None)
    plaintext = decrypted_bytes.decode("utf-8")
    return plaintext

# ====================================================================================================================
# Main Execution Block
# ====================================================================================================================
if __name__ == "__main__":
    test = "Hello, Encrypted World!"
    # print original text
    print(f"original text = {test}")
    # encrypt the text message
    nonce, ciphertext = encrypt_message(test, SHARED_KEY)
    # print nonce and hex string
    print(f"nonce: length = {len(nonce)}, hex string = {nonce.hex()}")
    # print cyphertext length and hex string
    print(f"ciphertext: length = {len(ciphertext)}, hex string ={ciphertext.hex()}")
    # test decrypting with valid key
    decrypted_test = decrypt_message(nonce, ciphertext, SHARED_KEY)
    # print decrypted text
    print(f"decrypted text = {decrypted_test}")
    # =================================================================================================================
    # Tamper Test
    # =================================================================================================================
    tampered = bytearray(ciphertext)
    # intentionally alter 1 byte of the ciphertext
    tampered[0] ^= 0x01
    # wrap the decrypt call in a try...except InvalidTag:
    try:
        decrypted = decrypt_message(nonce, tampered, SHARED_KEY)
        print(f"Decrypted: {decrypted}")
    except InvalidTag:
        print("Integrity check passed: Tampered message rejected!")

