# Encryption Tool

## Description

This tool provides a portable, lightweight, and bloatware free interface for easy encryption and decryption of sensitive data on the Windows platform.

## Features

* Highly secure reversible password based implementation of the advanced encryption standard (AES) for safely storing sensitive files on local drives.
* Random password salting and PBKDF2 key derivation to generate unique permutations for every encryption.
* Hash based verification for each and every write operation to ensure data can always be recovered.
* Secure memory release of sensitive information to reduce the risk recovery from memory dumps.
* Secure erasure of original files from drives to prevent recovery without passwords. 

## Usage (CLI application)

Encrypt a file:

```c#
encrypt "C:\Users\Elorith\Documents\FileThatIWantToEncrypt.txt"
```

Decrypt a file:
```c#
decrypt "C:\Users\Elorith\Documents\FileThatIWantToDecrypt.aes"
```

Erase a file:
```c#
erase "C:\Users\Elorith\Documents\FileThatIWantToErase.txt"
```

Exit the application:
```c#
exit
```
