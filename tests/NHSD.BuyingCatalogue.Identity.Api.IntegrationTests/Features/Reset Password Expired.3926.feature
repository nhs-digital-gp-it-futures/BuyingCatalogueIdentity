Feature: Reset Password Expired
    As a User
    I want to view the reset password expired page
    So that I can request a new reset password link

Background: 
    When the user navigates to identity url account/resetpasswordexpired
    Then the user is redirected to page account/resetpasswordexpired

@3926
Scenario: 1. Clicking the request new link button sends the user to the forgot password page
    When element with Data ID request-new-link-button is clicked
    Then the user is redirected to page identity/account/forgotpassword
