# SOApiProject

SOApiProject is a project aimed at providing an API interface for managing tags extracted from Stack Overflow data. This API facilitates various operations related to tags such as fetching tag data, calculating tag share, sorting tags, and repopulating the database with new tags.

## Project Goal

The primary goal of SOApiProject is to provide platform for managing Stack Overflow tags data. It aims to offer functionalities for retrieving tag information, analyzing tag distribution, and updating the database with fresh tag data.

## Endpoints

### GetTagsShare

This endpoint retrieves the share of tags in the database.

- **URL:** `GET /api/so/tags/share`
- **Response:** Returns a dictionary containing the share of each tag.

### GetTags

Retrieves tags with pagination and sorting options.

- **URL:** `GET /api/so/tags`
- **Query Parameters:**
    - `sortBy`: Specifies the field to sort by (`Name` or `Count`). Default is `Name`.
    - `ascending`: Specifies sort order. Default is `true`.
    - `pageSize`: Specifies the size of each page. Default is `100`.
- **Response:** Returns a list of paged tag models.
- **Error Responses:**
    - 400 Bad Request: Sorting can only be done by Name or Count.
    - 404 Not Found: No tags found.

### RepopulateDatabase

Repopulates the database with tags.

- **URL:** `POST /api/so/tags/repopulate`
- **Query Parameters:**
    - `tagsAmount`: Specifies the number of tags to generate. Default is `1000`.
- **Response:** Returns 200 if successful.
- **Error Response:**
    - 400 Bad Request: Tags amount cannot be less than 1000.