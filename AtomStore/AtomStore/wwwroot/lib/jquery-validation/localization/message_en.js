jQuery.extend(jQuery.validator.messages, {
        required: abp.localization.localize("FormValidationMessageRequired"),//"This field is required.",
        remote: "Please fix this field.",
        email: abp.localization.localize("FormValidationMessageEmail"),//"Please enter a valid email address.",
        url: abp.localization.localize("FormValidationMessageUrl"),//"Please enter a valid URL.",
        date: abp.localization.localize("FormValidationMessageDate"),//"Please enter a valid date.",
        dateISO: "Please enter a valid date (ISO).",
        number:  abp.localization.localize("FormValidationMessageNumber"),//"Please enter a valid number.",
        digits: "Please enter only digits.",
        creditcard: "Please enter a valid credit card number.",
        equalTo:  abp.localization.localize("FormValidationMessageDataEquals"),//"Please enter the same value again.",
        accept: "Please enter a value with a valid extension.",
        maxlength: jQuery.validator.format("Please enter no more than {0} characters."),
        minlength: jQuery.validator.format(abp.localization.localize("FormValidationMessageMinlength")),//jQuery.validator.format("Please enter at least {0} characters."),
        rangelength: jQuery.validator.format("Please enter a value between {0} and {1} characters long."),
        range: jQuery.validator.format("Please enter a value between {0} and {1}."),
        max: jQuery.validator.format(abp.localization.localize("FormValidationMessageMax")),//jQuery.validator.format("Please enter a value less than or equal to {0}."),
        min: jQuery.validator.format(abp.localization.localize("FormValidationMessageMin"))//jQuery.validator.format("Please enter a value greater than or equal to {0}.")
    });