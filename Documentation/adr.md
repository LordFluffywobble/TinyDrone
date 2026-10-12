# Architecture Analysis


### 1. No user specific options

### 2. No limit of how many drones a user can create in the logic

### 3. Password saved in plaintext in a json file

---

###                      Problem 1: User Options
#### This is a problem because we can not add, remove or give persmissions to a user.
**Priority: Medium.** 

We currently have a password on the database and only one user.


###                      Problem 2: Drone logic
#### No limit on drone creation could be used to overload the system and make it sluggish for others.
**Priority: Low.** 

Low importance of database as a target. 

*We need to make something to either track the number of drone operations 
or track how many drone operations are being done in any given time. Potentially per user.*



###                      Problem 3: Password woes
#### Hvaing a password in plaintext is remarkably unsafe and insecure.
**Priority: High**

It is saved in plaintext in a json file using <UserSecretId>. 
Can be retrieved using the command dotnet: user-secrets list.
UserSecrets is only meant for local development and is not used at all in production systems.

*To change this we need to make an option to create users who have different permissions.*

---

## 3. Potential solutions to my Password woes
**Alternative A:**
*Keep it to one user and run an instance for each user instead.*

Should be simple, spin up a docker for each user and manage through that.

Should be simble to terminate a user who is using too many resources, but more diffcult to simply throttle the number of drones created and used.

Relatively secure and easy to maintain. Needs to check for number of dockers spinning up at once.

**Alternative B:**
*Add the option of multiple users a and use OAuth etc for identification.*

A little more work in the program itself, but a better option. Add GUID, OAuth etc and track users and their resource use directly in the program.

The logic to do it is relatively simple and it's not too much work. Easier to add features to the program and have more granular control.

Security will also be a bit better if we implement it. Might even be better if we combine both solutions so we get the option to simply nuke a user if needed.

## Going with option B is the best path forward ##
**The ability for granular control, easy of expanding the program and applying tests to the code is a much better option than a simple Docker solution**
