FROM mcr.microsoft.com/dotnet/sdk:8.0

RUN wget https://aka.ms/getvsdbgsh && \
    sh getvsdbgsh -v latest  -l /vsdbg

### FastText
RUN apt -y update
RUN apt -y install curl
RUN mkdir /models -p
RUN curl --location -o /models/fasttext176.bin https://dl.fbaipublicfiles.com/fasttext/supervised-models/lid.176.bin
# RUN curl --location -o /models/fasttext217.bin https://huggingface.co/facebook/fasttext-language-identification/resolve/main/model.bin?download=true
###

### MediaPipe 
RUN apt -y update
RUN apt -y install curl
RUN curl --location -o /models/mediapipe_language_detector.tflite https://storage.googleapis.com/mediapipe-models/language_detector/language_detector/float32/1/language_detector.tflite
###


