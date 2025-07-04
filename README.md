[Requirements for the test](https://github.com/cmapsoftware/Technical-Test)

# How to run
- prerequisite: Docker installed
- After cloning the repo, run ```docker-compose up --build```
- For swagger: [http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html)

# Assumptions made
- Authentication was out of scope
- Unit testing alone is acceptable, other types of testing, such as stress testing, are out of scope
- It was not required to manage users and projects, just the timesheets
- Frontend was not required, just an API

# Features
1. **Add a new timesheet entry** ✅
2. **Edit an existing entry** ✅
3. **Delete an entry** ✅
4. **List all entries for a given user and week** ✅
5. **Get total hours per project for a given user and week** ✅
6. Get timesheet by Id ✅
7. Prevent duplicate entries for the same user, project, and date. ❌ (For senior applicants)

# Technologies Used
1. .NET 8
    - I chose this because it's the latest stable LTS version of .NET
2. Controllers rather than MinimalAPI
    - A smaller project like this, I would usually consider MinimalAPIs for simplicity
    - I chose controllers for this because it is more often used and likely what is used at CMAP
3. In Memory Lists rather than In Memory db or SQLite
    - A requirement stated in the requirements doc
    - Used an interface and DI so it can be swapped out for other implementations if moved to production db
4. xUnit
    - A modern and commonly used testing framework. Other valid options would have been NUnit
5. Docker
    - To make sure it runs the same on my machine than on yours
    - Commonly used container software and can simplify deployment and managing application dependencies

# Testing
In the ```/API``` directory run:
```dotnet test```
The organisation of the unit tests could have been vastly improved in my code. \
\
If I was to continue working on this I would:
- Split out the tests into separate files for each area testing
- Separate out test data so they can be reused across different tests and for better organisation
- I should have used [Theory] and [InlineData] for test data
- I used hardcoded GUIDs because I thought it would make tests more predictable but Guid.NewGuid() would have been fine

# Improvements/Refactors
- I would usually have a service between controllers and data access but because it was a smaller project it felt like abstraction for the sake of it, however some things should have been pulled out
  - Validation done in the repository could be methods in a validation util class can be called in a service before making it to the repo
- Timesheet update requires the whole timesheet, it might be better to make fields optional so partial data can be passed to the update endpoint
- As the project grows in volume and complexity it would be good to separate things out into different projects more. I considered this but it seemed overkill
  - Timesheet.API
  - Timesheet.Repository / DataAccess (commands and queries)
  - Timesheet.Core (models and interfaces)

# Thanks
Thank you for taking the time to review my solution to the tech test and for the opportunity to interview at CMAP. \
I am looking forward to hearing your feedback.
