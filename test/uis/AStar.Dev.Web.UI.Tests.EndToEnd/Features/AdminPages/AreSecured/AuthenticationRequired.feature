Feature: Admin pages require authentication

    Scenario: All Admin pages require authentication
        Given I am not logged in to the site
        When I visit any of the following pages:
          | PageName                  | Url                       |
          | Authentication Check      | authentication-check      |
          | add-files-to-database     | add-files-to-database     |
          | api-usage                 | api-usage                 |
          | models-to-ignore          | models-to-ignore          |
          | scrape-directories        | scrape-directories        |
          | search-configuration      | search-configuration      |
          | site-configuration        | site-configuration        |
          | site-configuration-update | site-configuration-update |
          | tags-to-ignore            | tags-to-ignore            |
        Then I am asked to login
