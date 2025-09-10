
# Slime Simulator

Slime sim is a GPU agent simulaton. 
Eac agent is represented as a coloured pixel on a texteue and moves around with a given speed and direction.
Depending on their properties they will make all sorts of patters when attempting to follow the places where the trail is brightest.
---

## How To Run
1. Download the build directory as .zip from (./Build)
2. Extract the zip file.  
3. Run `SlimeSim.exe`.  
4. Play around with the settings to see what you can make. (For a more detailed explaination of what the parameters do see below)
   
---
<p align="center">
  <img src="https://via.placeholder.com/200" width="200" />
  <img src="https://via.placeholder.com/200" width="200" />
  <img src="https://via.placeholder.com/200" width="200" />
  <img src="https://via.placeholder.com/200" width="200" />
</p>

---

## Features
- Image Viewer  
  - Load images in multiple formats (JPG, PNG, BMP, TIFF)
  - Import images from scanner using WIA 
  - Zoom with the mouse wheel  
  - Pan with right-click drag  

- Crop Tool  
  - Draw and adjust selection rectangles  
  - Preview cropped image  

- OCR (Text Recognition)  
  - Powered by [Tesseract OCR](https://github.com/tesseract-ocr/tesseract)  
  - Preprocessing images to improve recognition accuracy  
  - Extracts characters (A-Z, 0-9, -, _)  

- File Saving  
  - Easily save cropped images using the parsed lot number as filename  
  - Supports multiple output formats (JPG, PNG, BMP)  
  - Save location can be set once and reused  



---

## Development
1. Clone the repository and open it in Visual Studio 2019:
2. Right click solution and "Restone NuGet Packages"

```bash
git clone https://github.com/yourusername/CoCSaver.git
