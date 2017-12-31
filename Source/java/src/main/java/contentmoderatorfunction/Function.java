package contentmoderatorfunction;

import java.io.*;
import java.net.*;
import java.util.List;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;

import com.microsoft.azure.serverless.functions.annotation.*;
import com.microsoft.azure.serverless.functions.*;

import contentmoderatorfunction.vision.AnalysisResult;

/**
 * Azure Functions with Queue Trigger.
 */
public class Function {
    @FunctionName("reviewTextAndImage")
    public void reviewTextAndImage(
            @QueueTrigger(name = "queueInput", queueName = "review-queue", connection = "AzureWebJobsStorage") ReviewRequestItem queueInput,
            @BlobInput(name = "image", path = "input-images/{BlobName}", dataType = "binary", connection = "AzureWebJobsStorage") byte[] image,
            //@DocumentDBInput(name = "inputDocument", databaseName = "customerReviewData", collectionName = "reviews", id = "{DocumentId}", connection = "customerReviewDataDocDB") ImageDocument inputDocument,
            final ExecutionContext context) {

        context.getLogger().info("Java HTTP trigger processed a request.");

        //boolean passesText = PassesTextModerator(inputDocument);
        ImageInformation imageInformation = PassesImageModeration(image);

        //inputDocument.setIsApproved(imageInformation.getContainsCat() && passesText);
        //inputDocument.setCaption(imageInformation.getMessage());
    }

    private final String CONTENT_MODERATOR_API_URL = "https://westeurope.api.cognitive.microsoft.com/contentmoderator/moderate/v1.0/ProcessText/Screen?language=eng";
    private final String COMPUTER_VISION_API_URL = "https://westeurope.api.cognitive.microsoft.com/vision/v1.0/analyze?visualFeatures=Description,Tags&language=en";

    private boolean PassesTextModerator(ImageDocument document) {
        if (document.getReviewText() == null) {
            return true;
        }

        try {

            URL url = new URL(CONTENT_MODERATOR_API_URL);
            HttpURLConnection con = (HttpURLConnection) url.openConnection();
            con.setRequestMethod("POST");

            con.setRequestProperty("Ocp-Apim-Subscription-Key", System.getenv("ContentModerationApiKey"));

            con.setDoOutput(true);
            DataOutputStream out = new DataOutputStream(con.getOutputStream());
            out.writeBytes(document.getReviewText());
            out.flush();
            out.close();

            BufferedReader in = new BufferedReader(new InputStreamReader(con.getInputStream()));
            String inputLine;
            StringBuffer content = new StringBuffer();
            while ((inputLine = in.readLine()) != null) {
                content.append(inputLine);
            }
            in.close();

            con.disconnect();

            JsonNode result = new ObjectMapper().readTree(content.toString());

            return result.get("Terms") == null;

        } catch (Exception e) {
            return false;
        }
    }

    private ImageInformation PassesImageModeration(byte[] image) {
        try {

            URL url = new URL(COMPUTER_VISION_API_URL);
            HttpURLConnection con = (HttpURLConnection) url.openConnection();
            con.setRequestMethod("POST");

            con.setRequestProperty("Ocp-Apim-Subscription-Key", System.getenv("MicrosoftVisionApiKey"));
            con.setRequestProperty("Content-Type", "application/octet-stream");

            con.setDoOutput(true);
            DataOutputStream out = new DataOutputStream(con.getOutputStream());
            out.write(image);
            out.flush();
            out.close();

            BufferedReader in = new BufferedReader(new InputStreamReader(con.getInputStream()));
            String inputLine;
            StringBuffer content = new StringBuffer();
            while ((inputLine = in.readLine()) != null) {
                content.append(inputLine);
            }
            in.close();

            con.disconnect();

            AnalysisResult result = new ObjectMapper().readValue(content.toString(), AnalysisResult.class);

            List<String> tags = result.getDescription().getTags();
            boolean containsCat = false;

            for (String tag : tags) {
                if (tag.contains("cat")) {
                    containsCat = true;
                    break;
                }
            }

            String message = result.getDescription().getCaptions().get(0).getText();

            return new ImageInformation(containsCat, message);

        } catch (Exception e) {
            return null;
        }
    }

    private static class ImageDocument {
        private String ReviewText;
        private boolean IsApproved;
        private String Caption;

        public String getReviewText() {
            return ReviewText;
        }

        public void setIsApproved(boolean isApproved) {
            this.IsApproved = isApproved;
        }

        public void setCaption(String caption) {
            this.Caption = caption;
        }
    }

    private static class ImageInformation {
        private boolean containsCat;
        private String message;

        public ImageInformation(boolean containsCat, String message) {
            this.containsCat = containsCat;
            this.message = message;
        }

        public boolean getContainsCat() {
            return containsCat;
        }

        public String getMessage() {
            return message;
        }
    }

    private static class ReviewRequestItem {
        private String DocumentId;
        private String BlobName;
    }
}
