# =================================================================================================
# IMPORTS
# =================================================================================================
import os
# Packing/Unpacking the 4-byte big-endian length header >I
import struct
import socket
from cryptography.hazmat.primitives.ciphers.aead import AESGCM
from cryptography.exceptions import InvalidTag

# =================================================================================================
# CONSTANT
# =================================================================================================
SHARED_KEY = b"12345678901234567890123456789012"

# =================================================================================================
# FUNCTIONS
# =================================================================================================
# Read Exact Number of Bytes
def recv_exact(sock: socket.socket, n: int) -> bytes | None:
    data = bytearray()
    while len(data) < n:
        packet = sock.recv(n - len(data))
        if not packet:
            return None
        data.extend(packet)
    return bytes(data)

# Send Encrypted Message
def send_encrypted_message(sock: socket.socket, plaintext: str, key: bytes) -> None:
    plaintext_bytes = plaintext.encode("utf-8")
    nonce = os.urandom(12)
    aesgcm = AESGCM(key)
    encrypted_raw = aesgcm.encrypt(nonce, plaintext_bytes, associated_data=None)
    tag = encrypted_raw[-16:]
    ciphertext = encrypted_raw[:-16]
    payload = nonce + tag + ciphertext
    header = struct.pack(">I", len(payload))
    sock.sendall(header + payload)

# Recieve Encrypted Message
def recv_encrypted_message(sock: socket.socket, key: bytes) -> str | None:
    header = recv_exact(sock, 4)
    if header is None:
        return None
    (payload_len,) = struct.unpack(">I", header)
    payload = recv_exact(sock, payload_len)
    if payload is None:
        return None
    nonce = payload[0:12]
    tag = payload[12:28]
    ciphertext = payload[28:]
    combined = ciphertext + tag
    try:
        decrypted_bytes = AESGCM(key).decrypt(nonce, combined, None)
        return decrypted_bytes.decode("utf-8")
    except InvalidTag:
        print("[INTEGRITY ALERT] Authentication tag mismatch! Message was tampered with or corrupted.")
        return "[CORRUPTED MESSAGE - REJECTED]"

# =================================================================================================
# SELF-TEST ENTRY POINT
# =================================================================================================
if __name__ == "__main__":
    # Create an in-memory pair of connected sockets
    sender, reciever = socket.socketpair()

    test_message = "Protocol test: Hello Encrypted World!"
    print(f"[TEST] Sending: {test_message}")

    # Send encrypted packet
    send_encrypted_message(sender, test_message, SHARED_KEY)

    # Recieve and decrypt packet
    recieved_message = recv_encrypted_message(reciever, SHARED_KEY)
    print(f"[TEST] Recieved: {recieved_message}")

    assert test_message == recieved_message, "Mismatch!"
    print("[TEST] SUCCESS! Framing, Nonce, Tag, and Ciphertext round-trip verified.")

    sender.close()
    reciever.close()

