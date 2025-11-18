def validate_email(email):
    return '@' in email

def validate_age(age):
    return age > 0 and age < 150
