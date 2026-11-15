## If Essentials does not work

`Assets/Plugins/Essencial` is a symlink to a local clone of [EgorexW/Essentials](https://github.com/EgorexW/Essentials).
Use it locally in one of two ways:

1. Point the symlink at your local clone of the repo:
   ```
   git clone https://github.com/EgorexW/Essentials.git "<path to clone>"
   rm "Assets/Plugins/Essencial"
   ln -s "<path to clone>" "Assets/Plugins/Essencial"
   ```
2. Or delete the symlink and put the repo there directly as a real folder:
   ```
   rm "Assets/Plugins/Essencial"
   git clone https://github.com/EgorexW/Essentials.git "Assets/Plugins/Essencial"
   ```
