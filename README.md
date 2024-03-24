# dotInstrukcije

Za bazu sam koristila MongoDB - za testiranje je potrebno napraviti kolekcije Student, Professor, Subject, InstructionsDate

Entiteti

Student
id: id
email: String
name: String
surname: String
password: String
profilePictureUrl: String

Professor
email: String
name: String
surname: String
password: String
profilePictureUrl: String
instructionsCount: Number // broj održanih instrukcija
subjects: String[] // lista urlova/id-eva predmeta na kojima profesor predaje

Subject
id: id
title: String
url: String
description: String
Instructions Date
-studentId: id -professorId: id -dateTime: DateTime -status: String //status može biti poslan zahtjev, nadolazeća instrukcija ili prošla instukcija

InstructionsDate
id: id
studentId: string
email: string
date: DateTime
status: string

API Documentation

Student Routes

Register a Student
URL: /register/student
Method: POST
Description: Registers a new student by providing their details.
Body:
email: example@example.com
name: First
surname: Last
password: yourpassword
profilePictureUrl: https://example.com/profile.jpg
Returns:
success: true/false
message: string

Login a Student
URL: /login/student
Method: POST
Description: Authenticates a student using their email and password.
Body:
email: example@example.com
password: yourpassword
Returns:
success: true/false
student: student object (if success is true)
token: JWT token (if success is true)
message: string

Get a Specific Student by Email
URL: /student/:email
Method: GET
Description: Retrieves a specific student by their email. Requires JWT authentication.
Authentication: Bearer Token (JWT)
Returns:
success: true/false
student: student object (if success is true)
message: string

Get All Students
URL: /students
Method: GET
Description: Retrieves a list of all students. Requires JWT authentication.
Authentication: Bearer Token (JWT)
Returns:
success: true/false
students: array of student objects (if success is true)
message: string

Professor Routes

Register a Professor
URL: /register/professor
Method: POST
Description: Registers a new professor by providing their details.
Body:
email: example@example.com
name: First
surname: Last
password: yourpassword
profilePictureUrl: https://example.com/profile.jpg
subjects: [Math, Science]
Returns:
success: true/false
message: string

Login a Professor
URL: /login/professor
Method: POST
Description: Authenticates a professor using their email and password.
Body:
email: example@example.com
password: yourpassword
Returns:
success: true/false
professor: professor object (if success is true)
token: JWT token (if success is true)
message: string

Get a Specific Professor by Email
URL: /professor/:email
Method: GET
Description: Retrieves a specific professor by their email. Requires JWT authentication.
Authentication: Bearer Token (JWT)
Returns:
success: true/false
professor: professor object (if success is true)

Get All Professors
URL: /professors
Method: GET
Description: Retrieves a list of all professors. Requires JWT authentication.
Authentication: Bearer Token (JWT)
Returns:
success: true/false
students: array of professor objects (if success is true)
Subject Routes (Requires JWT authentication)

Create a Subject
URL: /subject
Method: POST
Description: Creates a new subject with a unique title.
Authentication: Bearer Token (JWT)
Body:
title: Unique title for the subject
url: Unique URL
description: Description of the subject
Returns:
success: true/false
message: string

Get a Subject by URL
URL: /subject/:url
Method: GET
Description: Retrieves a specific subject by its title.
Authentication: Bearer Token (JWT)
Returns:
success: true/false
subject: subject object (if success is true)
professors: list of associated professors (if success is true)
message: string

Get All Subjects
URL: /subjects
Method: GET
Description: Retrieves a list of all subjects.
Returns:
success: true/false
subjects: array of subject objects (if success is true)
Schedule Instruction Session
URL: /instructions
Method: POST
Authentication: Bearer Token (JWT)
Request Body:
date [required]: The date selected for the instruction session.
professorId [required]: The unique identifier of the professor with whom the session is to be scheduled.
Returns:
success: true/false
message: string

-- također su dodane rute koje služe za Update studenata/profesora u bazi -> za stranicu Postavke na frontu.

- funkcioniraju Registracija i Login (registracija je moguća jedino uz email koji nije već u bazi), implementirana je i JWT autorizacija. Dodana je stranica Postavke te se je preko nje omogućeno mijenjanje podataka. Omogućena je i rezervacija instrukcija, međutim nije uključeno odobravanje instrukcije od strane profesora (ali može se testirati kroz bazu, ako se ažurira status na "pastInstructions" ili "upcomingInstructions"). Na stranici Moje instrukcije vidljive su poslane, prošle i nadolazeće instrukcije. Za Subjects je omogućeno dodavanje i dohvaćanje, ali nije razrađena logika povezivanja Profesora i predmeta s instrukcijom...
