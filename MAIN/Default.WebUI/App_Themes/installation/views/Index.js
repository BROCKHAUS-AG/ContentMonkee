var KnockoutMixin = {

    updateKnockout() {
        this.__koTrigger(!this.__koTrigger());
    },

    componentDidMount() {
        this.__koTrigger = ko.observable(true);
        this.__koModel = ko.computed(function () {
            this.__koTrigger(); // subscribe to changes of this...

            return {
                props: this.props,
                state: this.state
            };
        }, this);

        ko.applyBindings(this.__koModel, ReactDOM.findDOMNode(this));
    },

    componentWillUnmount() {
        ko.cleanNode(ReactDOM.findDOMNode(this));
    },

    componentDidUpdate() {
        this.updateKnockout();
    }
};

var Data = {
    User: {
        UserName: ko.observable(''),
        FirstName: ko.observable(''),
        LastName: ko.observable(''),
        Password: ko.observable(''),
        RPassword: ko.observable(''),
        Mail: ko.observable(''),

        UserNameValid: function () {
            return isValid(Data.User.UserName());
        },
        FirstNameValid: function () {
            return isValid(Data.User.FirstName());
        },
        LastNameValid: function () {
            return isValid(Data.User.LastName());
        },
        PasswordValid: function () {
            return isValidPassword(Data.User.Password());
        },
        RPasswordValid: function () {
            return Data.User.RPassword() == Data.User.Password();
        },
        MailValid: function () {
            return isValidMail(Data.User.Mail());
        },

        isValid: function () {
            var user = Data.User;
            return user.UserNameValid() &&
                user.FirstNameValid() &&
                user.LastNameValid() &&
                user.PasswordValid() &&
                user.RPasswordValid() &&
                user.MailValid();
        }

    },
    Settings: {
        Name: ko.observable(''),
        Domain: ko.observable(''),
        
        NameValid: function () {
            return isValid(Data.Settings.Name());
        },
        DomainValid: function () {
            return true;
        },

        isValid: function () {
            var settings = Data.Settings;
            return settings.NameValid() &&
                settings.DomainValid();
        }

    }
}


var DomainBinding = ko.computed(function () {
    return "localhost, " + Data.Settings.Domain();
}, this)

Data.User.UserName.subscribe(function (value) {
    Data.Settings.Autor = value;
});

InstallData = function () {
    $.ajax({
        url: "/Installation/Install/",
        type: 'post',
        data: ko.toJSON(Data),
        contentType: 'application/json',
        success: function (result) {
            $('#message').html(result);
        }
    });
}



//ko.applyBindings(Data);