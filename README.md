# 🧠 Object Detection Web App

Detects objects in uploaded images, analyzes dominant colors, and stores this data in Elasticsearch, allowing users to search based on labels, color, and city. Built with a modern Clean Architecture approach using .NET, the application communicates with an external AI service via HTTP to deliver powerful image analysis capabilities.

---

## 🚀 Project Goal

This application manages an end-to-end process starting from image upload:

1. An image is uploaded.
2. It is sent to an external AI service via HTTP POST.
3. The returned analysis results are stored in both a database and Elasticsearch.
4. Users can search images using label and color information.

---

## 🧱 Architecture Overview (Clean Architecture)

```
ObjectDetection/
├── Core/
│   ├── Application     → UseCases, DTOs, service contracts
│   └── Domain          → Entities and core business rules
├── Infrastructure/
│   ├── Infrastructure  → External integrations such as AI service
│   └── Persistence     → EF Core-based data access
├── Presentation/       → .NET interface
└── docker/
    └── docker-compose.yml → Elasticsearch + Kibana
```

---

## 🛠 Technologies Used

* ✅ .NET 9 
* ✅ Entity Framework Core + MS SQL Server
* ✅ Clean Architecture principles
* ✅ HTTP-based AI service integration
* ✅ Elasticsearch 7.17 + Kibana
* ✅ Docker Compose (for Elastic stack)

---

## ⚙️ Setup

### 🔽 1. Clone the Project from GitHub

```bash
git clone https://github.com/emrecankahraman/ObjectDetection.git
cd ObjectDetection
```

> You can open the project using Visual Studio or Rider.

### 2. Requirements

* [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
* [Docker Desktop](https://www.docker.com/products/docker-desktop)
* A working instance of MS SQL Server

### 2. Start Elasticsearch + Kibana

```bash
cd docker
docker-compose up -d
```

* Elasticsearch: [http://localhost:9200](http://localhost:9200)
* Kibana: [http://localhost:5601](http://localhost:5601)

### 3. Database Setup

Before creating migrations, remove any previous migration folders if they exist:

```bash
# Delete the Migrations folder under ObjectDetection.Persistence
```

To create and apply a new migration:

```bash
dotnet ef migrations add InitialCreate -p ObjectDetection.Persistence -s ObjectDetection.Presentation
dotnet ef database update -p ObjectDetection.Persistence -s ObjectDetection.Presentation
```

### 4. Run the Application

```bash
cd ObjectDetection.Presentation
dotnet run
```

📌 The connection string in `appsettings.json` should be configured like this:

```json
"ConnectionStrings": {
  "DefaultConnection": "yourConnectionString"
}
```

Ensure this connection string matches your local SQL Server instance.

---

## 🧪 Application Flow

1. 👤 Register / Login
2. 📤 Upload an image
3. 🔁 The image is sent to an external AI service via HTTP
4. 🧠 Detected objects and colors are stored in:

   * The database (via EF Core)
   * Elasticsearch (for search functionality)
5. 🔍 Example search queries:

   * "white car istanbul"
   * "black dog"

---

## 🧠 How the AI Service Works

The uploaded image is sent to an external AI service via the `FlaskAIClient` class. The service:

 * Location Extraction (if available): The service first checks the image’s EXIF metadata for GPS coordinates. If found, it extracts geolocation details such as country, city, state, and street.

* OpenCV Conversion: The image is decoded and processed using OpenCV.

* Preprocessing: preprocess_image() enhances the image's color saturation to improve detection and color accuracy.

* Object Detection: TensorFlow’s Faster R-CNN model is used to detect objects within the image.

* Cropping: Detected objects are cropped individually from the original image.

* Background Removal: Each cropped object is sent to the remove.bg API to remove the background.

* Dominant Color Analysis: KMeans clustering is applied to the masked and preprocessed image to determine the dominant color for each object.

* Result Output: The final results, including detected classes, confidence scores, dominant colors, and location information, are returned as a structured JSON.

This communication is done via `HTTP POST`. The response is parsed into `DetectionResultDto` objects and stored in both the database and Elasticsearch.

🔗 For the source code of the AI service: [object-detection-ai](https://github.com/emrecankahraman/ObjectDetectionPythonProject)

---

## 🐳 Docker Content

`docker/docker-compose.yml` is used to run Elasticsearch and Kibana:

```yaml
elasticsearch:
  image: elasticsearch:7.17.10
  ports: ["9200:9200"]

kibana:
  image: kibana:7.17.10
  ports: ["5601:5601"]
```

---

## 📌 AI Service Configuration

The AI service endpoint is defined within the `FlaskAIClient` class in code. The default URL is:

```csharp
var url = _configuration["AIService:Url"] ?? "http://127.0.0.1:5000/predict";
```

You can override this by defining `AIService:Url` in `appsettings.json`. Otherwise, the default address will be used.

---

## 📄 License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).
