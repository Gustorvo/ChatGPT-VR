![ChatGPT-Demo](https://user-images.githubusercontent.com/31797378/232576278-23099524-f0c9-426c-857f-9139ba604267.gif)

# ChatGPT-VR Documentation
ChatGPT-VR is a Unity VR project that allows users to interact with a virtual assistant using voice commands. The virtual assistant is powered by OpenAI's GPT-3 model (can be changed to any other model) and features [STT](https://azure.microsoft.com/en-us/products/cognitive-services/speech-to-text/) (speech-to-text) and [TTS](https://azure.microsoft.com/en-us/products/cognitive-services/text-to-speech/) (text-to-speech) services from Azure Cognitive Services, a lipsync engine (modified and stripped version of [Oculus lipsync](https://developer.oculus.com/documentation/unity/audio-ovrlipsync-unity/)), an eye movement script and a simple head movement script. The connction to ChatGPT is handled by [OpenAI package](https://github.com/RageAgainstThePixel/com.openai.unity) via RESTful API. The 3D avatar model is taken from [ReadyPlayerMe](https://readyplayer.me/) and can be easily replace with your own.

## Installation and Setup
 - To run the ChatGPT-VR project, you will need to have Unity version 2021.3.15f1 or later installed. 
 - Additionally, you will need to have an [OpenAI API key](https://platform.openai.com/account/api-keys) and an [Azure Cognitive Services subscription key](https://azure.microsoft.com/en-us/free/cognitive-services/) to use the STT and TTS services.
To provide your API keys to Azure and OpenAI, navigate to `\Assets\Resources` and paste it directrly into respective scriptable objects.

![api key](https://user-images.githubusercontent.com/31797378/232571533-cf54a642-ef8b-4064-b3c1-803b4777dc8d.png)

- The project is using [Git LFS.](https://git-lfs.github.com/)

- Oculus (Meta) Quest headset is required (if you want to run in on VR device). Make sure to install any other XR plugins if you're going to use other HMD rather than Oculus.

## Usage and Functionality
Build the project for Meta (Oculus) Quest or run it directly in editor.
Once you run the project, you can interact with the virtual assistant using voice commands. The virtual assistant will respond to your commands using its voice. The avatar will move its lips and eyes to simulate natural conversation.

### Contributing
Contributions to the ChatGPT-VR project are welcome. To contribute, please fork the repository, make your changes, and submit a pull request.


## License
ChatGPT-VR is released under the MIT License. See the LICENSE file for more information.
