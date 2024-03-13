# KYC360

## Project Overview

This project is a sample implementation of an ASP.NET Core Web API for managing entities, such as individuals, in a database. The API allows users to perform CRUD (Create, Read, Update, Delete) operations on entities and includes features such as searching and filtering. Additionaly it also performs both the bonus challenges which includes paging, sorting and retry, backoff mechanism with logging.

## Retry and Backoff Strategy for Database Write Operations

### Implementation Overview:

The `EntityService` class handles database write operations such as adding, updating, and deleting entities. To enhance the robustness of the system against transient failures, a retry and backoff mechanism has been implemented. This mechanism automatically retries failed database write operations, with a progressively increasing delay between retry attempts.

### Retry Mechanism:

- The retry mechanism allows the API to automatically retry a failed database write operation.
- A maximum number of retry attempts (3) has been set before considering the operation as permanently failed.
- Retry attempts are made with an initial delay of 1 second, which increases exponentially with each retry, up to a maximum delay of 10 seconds.

### Backoff Strategy:

- An exponential backoff strategy has been chosen to prevent overwhelming the database with repeated requests during transient failures.
- This strategy increases the delay between retry attempts exponentially, providing more time for the transient issue to resolve.
- The initial delay is set to 1 second, and it doubles with each retry attempt, up to a maximum delay of 10 seconds.
- Exponential backoff strikes a balance between allowing the system to recover from transient failures and minimizing the impact on user experience.

### Rationale:

- **System Stability**: The retry and backoff strategy enhances the stability of the system by automatically handling transient failures without manual intervention. By retrying failed operations, the system increases the likelihood of successful completion, improving overall reliability.
- **User Experience**: Implementing a retry and backoff mechanism helps maintain a seamless user experience by minimizing the impact of transient failures on users. Rather than immediately returning an error to the user, the system makes multiple attempts to complete the operation, reducing the likelihood of user-facing errors.
- **Nature of Transient Failures**: Transient failures are often short-lived and temporary. By using an exponential backoff strategy, the system adapts to the transient nature of these failures, gradually increasing the delay between retries. This approach allows the system to gracefully handle transient issues without overwhelming the database or exacerbating the problem.

### Conclusion:

The implementation of retry and backoff strategy for database write operations in the `EntityService` class improves the resilience of the system against transient failures. By automatically retrying failed operations with a progressively increasing delay, the system can recover from transient issues while minimizing disruptions to user experience. This approach contributes to the overall stability and reliability of the system in real-world scenarios.

## Features

1. **CRUD Operations**: Perform Create, Read, Update, and Delete operations on entities.
2. **Search Functionality**: Search for entities based on names and addresses.
3. **Filtering**: Filter entities based on criteria such as gender, date of birth, and countries.
4. **Retry and Backoff Mechanism**: Automatically retry failed database write operations with an exponential backoff strategy to enhance system robustness.
5. **Pagination and Sorting**: Paginate the list of entities and sort them based on specified parameters.

## Technologies Used

- **ASP.NET Core**: Framework for building cross-platform web applications.
- **C#**: Primary programming language.
- **Swagger**: Tool for documenting and testing APIs.
- **Serilog**: Logging library for .NET applications.

## Setup Instructions

1. **Clone the Repository**: `git clone <repository-url>`
2. **Navigate to Project Directory**: `cd <project-directory>`
3. **Restore Dependencies**: `dotnet restore`
4. **Run the Application**: `dotnet run`

## Usage

1. **API Endpoints**: Interact with the API using endpoints such as `/entities`, `/entities/{id}`, `/entities/search`, `/entities/filter`, etc.
2. **Testing**: Utilize tools like Postman to send HTTP requests and test different functionalities.The http port is set to number 5077
3. **Example commands**:GET http://localhost:5077/entities, GET http://localhost:5077/entities?pageNumber=1&pageSize=10&sortBy=Id&sortOrder=asc, GET http://localhost:5077/entities/search?query=Monish

## Contribution Guidelines

1. **Fork the Repository**: Click on the Fork button on the repository's page.
2. **Clone Your Fork**: `git clone <your-fork-url>`
3. **Create a Branch**: `git checkout -b <branch-name>`
4. **Make Changes**: Implement new features, fix bugs, or improve documentation.
5. **Commit Changes**: `git commit -m "Descriptive commit message"`
6. **Push Changes**: `git push origin <branch-name>`
7. **Create Pull Request**: Go to your forked repository and create a pull request.


