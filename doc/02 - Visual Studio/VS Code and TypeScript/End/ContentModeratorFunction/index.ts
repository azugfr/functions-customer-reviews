import axios from 'axios';

const CONTENT_MODERATOR_API_URL: string = "https://westeurope.api.cognitive.microsoft.com/contentmoderator/moderate/v1.0/ProcessText/Screen?language=eng";
const COMPUTER_VISION_API_URL: string = "https://westeurope.api.cognitive.microsoft.com/vision/v1.0/analyze?visualFeatures=Description,Tags&language=en";

export async function run(context: any, queueInput: any, image: any, inputDocumentIn: any) {
    let passesText = await passesTextModeratorAsync(inputDocumentIn);

    context.bindings.inputDocumentOut = inputDocumentIn;

    let imageInformation = await analyzeImage(image);

    context.bindings.inputDocumentOut.IsApproved = imageInformation[1] && passesText;
    context.bindings.inputDocumentOut.Caption = imageInformation[0];
};

async function passesTextModeratorAsync(document: any): Promise<boolean> {
    if (document.ReviewText == null) {
        return true;
    }

    let config: any = {
        headers: {
            "Ocp-Apim-Subscription-Key": process.env["ContentModerationApiKey"],
            "Content-Type": "text/plain"
        }
    };

    let content: string = document.ReviewText;
    let result = await axios.post(
        CONTENT_MODERATOR_API_URL,
        content,
        config);

    // If we have Terms in result it failed the moderation (Terms will have the bad terms)
    return result.data.Terms == null;
}

async function analyzeImage(image: any): Promise<[string, boolean]> {
    let config: any = {
        processData: false,
        headers: {
            "Ocp-Apim-Subscription-Key": process.env["MicrosoftVisionApiKey"],
            "Content-Type": "application/octet-stream"
        }
    };

    let result = await axios.post(
        COMPUTER_VISION_API_URL,
        image,
        config);

    let caption = result.data.description.captions[0].text;
    let containsCat = (<Array<Tag>>result.data.tags).some(item => {
        return item.name.indexOf("cat") !== -1;
    });

    return [caption, containsCat];
}

interface Tag {
    confidence: number,
    name: string
}