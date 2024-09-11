- The web api has been configured with swagger and it can be accessed on http://localhost:5254/swagger
- Typically would have used SeriLog

How to add more sports?
- Each sport would have their own controller, processor and repository

How to add all NFL teams?
- The Nfl repository is currently keyed by <string position> however if the teams were included, it would be keyed by a tuple of <(string team, string position)>. This was not implemented as it would require the method signatures to be altered and accept another parameter of team
