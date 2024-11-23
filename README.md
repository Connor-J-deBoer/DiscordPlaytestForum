# Discord Playtest Forum

This a few basic scripts that allow for a scriptable object of questions (Designed for playtests, but applicatable elsewhere) to be created and used to send a response to a discord web hook

# How To Do It
- Right click in the Assets folder and go to Create > ScriptableObjects > Question List
- Fill the "Discord Web Hook URL" field with the webhook from a discord channel integration
- Add to the Questions list by pressing the "+" button
- Fill the Question Text with the question you want to ask
- Add an Answer, fill the Name with the answer text
- You can make the question required or allow more than 1 answer by selecting the correct bool underneath the answers
- Drag and drop the Question List into the UI manager

# UI Manager
The question list scriptable object is public and easy to add into a UI manager class. If you would like a basic solution there's a TextMeshPro and UI Toolkit solution included in the Samples.
