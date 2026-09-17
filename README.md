# VU Calendar v2

A web application that helps students build a personal course timetable, pick modules that don't clash with their schedule, and export the result to their own calendar app.
 
Implemented with: ASP.NET (REST API) and React
 
---

## User stories

### 1. View up-to-date timetable

As a user, I want to see an up-to-date timetable so that I can plan my course.
- Given I open the app, when the timetable loads, then it reflects the latest data available (not a copy older than the configured refresh interval).
- If the source data is temporarily unavailable, the app shows the last successfully loaded timetable with a visible "last updated" timestamp.

### 2. Pick a main course

As a user, I want to be able to pick my main course so that my timetable is built around it.
- Given a list of available courses, when I select one, then it becomes my active course and its lectures appear on my timetable.
- I can change my selected course at any time, which replaces the previous course's lectures.

### 3. Pick from available modules

As a user, I want to be able to pick from all available modules so that I can customize my schedule.
- When I select a module from a list it's added to my timetable alongside my main course.
- I can remove a previously selected module.

### 4. Detect overlaps between course and modules

As a user, I want to see if my picked course and modules overlap so that I can pick modules I can actually attend.
- When my selected course and a module overlap on the timetable the module is visually flagged as overlapping before I confirm the selection.

### 5. Search for a course

As a user, I want a search bar to find a specific course or module so that I don't have to scroll through the full list.
- When I type a course/module name into the course/module search bar the course list filters to matches in real time (or on submit).
- Searching is case-insensitive and matches partial names.

### 6. Filter by faculty

As a user, I want to filter courses and modules by faculty so that I only see options relevant to my department.
- Given a faculty filter, when I select one, then both the course and module lists update to show only that faculty's programs.

### 7. See non-overlapping modules

As a user, I want to see which modules do not overlap with my main course so that I can quickly find valid options.
- According to my selected main course, when I view the module list modules that don't clash with it are highlighted.

### 8. Export calendar

As a user, I want to export my calendar so that I can use it in my own calendar app.
- Given a finalized selection of course and modules, when I choose "Export," then I receive a valid `.ics` file containing all selected sessions.
- The exported file opens correctly in at least one common calendar app.

### 9. Log in to save calendar

As a user, I want to log in so that my calendar is saved and available when I return.
- Given valid credentials, when I log in, then my previously saved course/module selections load automatically.
- If I make changes while logged in, they persist across sessions without needing manual save.

### 10. Save modules as favorites

As a user, I want to save specific modules as favorites so that I can find them quickly without repeating my search.
- When I mark a module as a favorite it appears in a dedicated "Favorites" list.
- Favorites persist across sessions for logged-in users.

### 11. Get notified of schedule changes

As a user, I want to be notified if a module I've selected changes time or is cancelled so that my calendar stays accurate.
- If a module is in my calendar and its time changes in the source data, then I can see a notification on my next log-in.
- The notification identifies which module changed and what changed.
