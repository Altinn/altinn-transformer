# Altinn Transformer

The transformer is a Maskinporten-secured REST API that transforms both structured and unstructured values from various representations and formats. There are two principal applications:

* Exchange identifiers between various Altinn-specific and national formats/types, eg. a Altinn user-id to a 11-digit norwegian identification number
* Encrypt either identifiers or arbitrary data into a encrypted blob that is URL parameter safe, to allow for  transfer or sensitive data over unsecure channels between parties.

**This is a initial PoC, only implementing the encryption/decryption mechanism.**

For now, see https://digdir.atlassian.net/wiki/spaces/~6205262fa29402006879a50f/pages/edit-v2/2939977762 for further information