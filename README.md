# KaspaModule

Steps to make it work:
1. Install:
- [Git](https://git-scm.com/)
- [Git LFS](https://git-lfs.github.com/)
- Current version of Powershell for your operating system [Windows](https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-windows)/[Linux](https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-linux)/[MacOS](https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-macos)
- [.NET](https://dotnet.microsoft.com/en-us/download/dotnet) version 8.0 or up for your operating system
2. Clone the repository: 
```git
git clone https://github.com/KaspaForPowershell/KaspaModule
```
3. Create Powershell [profile](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_profiles) file.
4. Add to the Powershell profile path to `this-initialize-environment.ps1` file that you can find in this repository `.custom/Scripts` folder. Ensure that you prefix this path with a `.`, for example:   
```powershell
. "C:/Users/YOUR_USER_NAME/Documents/GIT/KaspaModule/.custom/Scripts/this-initialize-environment.ps1"
```

Now if you run a Powershell session, it should automatically compile and import module.

### Consider a donation to help support the project: [kaspa:qp5zyvkldad2702h2gj3fv674ehhcwe84ga9d5h5k65pg4jdcj7j5zsd9n9l7](https://www.kas.fyi/address/kaspa:qp5zyvkldad2702h2gj3fv674ehhcwe84ga9d5h5k65pg4jdcj7j5zsd9n9l7)
