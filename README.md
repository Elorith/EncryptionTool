# Encryption Tool

## Description

This encryption tool provides a portable, lightweight, and bloatware free interface for easy encryption and decryption of sensitive data on the Windows platform.

## Features

* Highly secure reversible password based implementation of the advanced encryption standard (AES) for safely storing sensitive files on local drives.
* Password salting and PBKDF2 key derivation to generate unique permutation for every encryption.
* Hash based verification for each and every write operation to ensure data can always be recovered.
* Secure memory overwriting to reduce the risk of sensitive information being recovered from memory dumps.
* Secure erasure of original files from drives to prevent recovery without passwords. 

## Usage (CLI application)

Encrypt a file:

```
encrypt "C:\Users\Elorith\Documents\MyFile.txt"
```

Decrypt a file:
```
decrypt "C:\Users\Elorith\Documents\31dcfe763b6676217d8ff213c0297721c71696df2251f4af4c188c01f34efa78.aes"
```

Exit the application:
```
exit
```
