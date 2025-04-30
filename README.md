# Less Annoying CRM Assessment

A telephony integration application for Less Annoying CRM that handles call events and matches callers with CRM contacts.

## Project Overview

This application provides a bridge between telephony systems and Less Annoying CRM. When a call event occurs, the API processes the event, communicates with Less Annoying CRM to find matching contacts, and returns the relevant contact information.

### Features

- Process telephony events through REST API
- Match callers with LACRM contacts based on phone number
- Log API requests for monitoring
- Angular-based admin interface for viewing logs

## Architecture

The project consists of two main components:

1. **Backend API** - ASP.NET Core Web API
2. **Frontend** - Angular application

### Technology Stack

- **.NET 8.0** - Backend framework
- **Entity Framework Core** - Data access with InMemory provider
- **Angular** - Frontend framework
- **Angular Material** - UI component library

## API Endpoints

### POST /api/calls

Processes a telephony event and returns matching contact information.

#### Request Body
```json
{
  "EventName": "incoming_call",
  "CallStart": "2023-04-12T10:30:00",
  "CallId": "call_12345",
  "CallersName": "John Doe",
  "CallersTelephoneNumber": "+15551234567"
}
```

#### Response
```json
{
  "contactId": "contact_12345",
  "name": "John Doe",
  "assignedTo": "User ID",
  "phone": "+15551234567",
  "isCompany": false
}
```

### GET /api/logs

Returns a log of recent API requests.

## Setup and Installation

### Prerequisites

- .NET 8.0 SDK
- Node.js and npm
- Angular CLI

### Backend Setup

1. Clone the repository
2. Navigate to the project root directory
3. Update `appsettings.json` with your LACRM API key
```json
{
  "Lacrm": {
    "ApiKey": "your-api-key-here"
  }
}
```
4. Run the API:
```
dotnet run
```

### Frontend Setup

1. Navigate to the LACRMAngularApp directory
2. Install dependencies:
```
npm install
```
3. Run the Angular development server:
```
ng serve
```

## Development

### Project Structure

- **Controllers/** - API endpoints
- **Services/** - Business logic
- **Repositories/** - Data access
- **Entities/** - Data models
- **Models/** - View models
- **Interfaces/** - Abstractions
- **LACRMAngularApp/** - Angular frontend

