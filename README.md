# Helmet-mounted Display (Aerial Vehicle)

## *HPVDT, University of Toronto*

## How to compile

- `git clone <this project> --recursive`
- `git checkout <development branch>`
- `git submodule update --init --recursive`
- Open this directory with the latest Unity editor 2022 LTS
- Select your favourite IDE in menu `Editor/Preferences/External Tools`
- Open with IDE by selecting `Assets/Open C# Project`

## How to contribute

- For every new feature or patch, always `git checkout <a new branch>`
- Edit and test your changes with your favourite tools, recommended tools are:
  - Unity Engine:
    - Eidtor 2022 LTS
    - Unity Package Manager
  - IDE:
    - JetBrains Riders
    - Visual Studio 2022
    - Visual Studio Code
  - Git GUI:
    - Also JetBrains Riders
    - GitKraken Pro
  - Documentation
    - Obsidian
- For all changes, `git add *` & `git commit`
- `git push` and submit a PR (pull request)
- To keep up your branch with the latest version, `git rebase` all commits of your branch with squashing
  - For beginners, this should be performed with a Git GUI

## Communication

- Project status: https://github.com/orgs/hpvdt/projects/2
- Feel free to propose new feature & hotfix as task list items
- Please be nice be respectful, particularly to all test pilots (Calvin, Bill, Claire) who are risking their lives to push the envelop

## How to ask questions

- Go to project status page: https://github.com/orgs/hpvdt/projects/2
- Select the component you intend to ask about (under `TODO` tab, enclosed in `[]`, e.g. `[Pilot UI]` and `[Telemetry]`)
- Find the owner/assignee of the component on the same page
- post your question under the same page, or ask the owner directly via Discord.