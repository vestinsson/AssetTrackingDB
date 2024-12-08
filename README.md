# AssetTrackingDB

# Asset Management Database System

This project is an asset management system implemented in C# using Entity Framework Core for database operations. The system allows for full CRUD, creation, reading, updating, and deletion of various types of assets, including laptops and mobile phones. It also includes features for assigning assets to offices and users, as well as generating statistics reports.

## Features

- **Asset Tracking**: Track different types of assets such as MacBooks, Asus Laptops, Lenovo Laptops, iPhones, Samsung Phones, and Nokia Phones.
- **Office Management**: Assign assets to different offices located in various countries.
- **User Management**: Assign assets to users and view assets by user.
- **Currency Conversion**: Convert asset prices to different currencies based on the office location.
- **Statistics Report**: Generate a statistics report showing total assets, asset value, assets by type, assets by office, and more.

## Setup

1. **Clone the Repository**:
   
   git clone <repository-url>
   cd <repository-directory>

2. **Install Dependencies**:
   Ensure you have the necessary dependencies installed. You can use NuGet Package Manager to install any missing packages.

3. **Database Configuration**:
   The system uses SQLite for database operations. Ensure the database file `AssetTrackingF.db` is created in the project directory.

4. **Run the Application**:
   Build and run the application using your preferred IDE or the command line.

## Usage

1. **Main Menu**:
   The main menu provides options to add new assets, view all assets, update assets, delete assets, view assets by office, view assets by user, view users and their assigned assets, and generate a statistics report.

2. **Adding Assets**:
   Select the type of asset to add (MacBook, Asus Laptop, Lenovo Laptop, iPhone, Samsung Phone, Nokia Phone) and enter the required details.

3. **Viewing Assets**:
   View all assets with sorting options or view assets assigned to a specific office or user.

4. **Updating Assets**:
   Select an asset to update and enter the new $ price.

5. **Deleting Assets**:
   Select an asset to delete from the system.

6. **Generating Reports**:
   Generate a statistics report to get insights into the asset management system.

## Code Structure

- **Enums**:
  - `AssetLifeStatus`: Enum to represent the life status of an asset (Normal, Yellow, Red, Grey).
  - `Country`: Enum to represent the country of an office (UnitedStates, Germany, Sweden).

- **Models**:
  - `Asset`: Abstract base class for all assets.
  - `MacBook`, `AsusLaptop`, `LenovoLaptop`: Subclasses for different types of laptops.
  - `Iphone`, `SamsungPhone`, `NokiaPhone`: Subclasses for different types of mobile phones.
  - `Office`: Class to represent an office with a collection of assets.
  - `User`: Class to represent a user with a collection of assigned assets.
  - `AssetUser`: Join entity to represent the many-to-many relationship between assets and users.

- **Database Context**:
  - `AssetTrackingContext`: DbContext class for database operations, including seeding initial data.

- **Services**:
  - `AssetManagementService`: Service class for CRUD operations on assets, including methods to assign assets to offices and users, and generate statistics reports.

- **Utilities**:
  - `CurrencyConverter`: Static class for currency conversion.
  - `DateTimeExtensions`: Static class for DateTime extensions.

- **Program**:
  - `Program`: Main class containing the entry point of the application and methods for the user interface.
  
## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contact

For any questions or issues, please open an issue on the GitHub repository.
```
