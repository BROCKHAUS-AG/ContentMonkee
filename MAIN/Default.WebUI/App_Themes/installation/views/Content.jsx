
var WelcomeContent = React.createClass({
    mixins: [KnockoutMixin],
    getInitialState: function () {
        return {
        };
    },
    render: function () {
        return (
            <div className="inst-outerdiv inst-welcome">
                <h1>Welcome</h1>
                <hr />
                <div className="inst-text">
                    <p>
                        Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.
                    </p>
                </div>
            </div>
            );
    }
});
var InstructionContent = React.createClass({
    mixins: [KnockoutMixin],
    getInitialState: function () {
        return {
        };
    },
    render: function () {
        return (
            <div className="inst-outerdiv inst-instruction">
                <h1>First instructions</h1>
                <hr />
                <div className="inst-text">
                    <p>
                        Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.
                    </p>
                </div>
            </div>
            );
    }
});

var UserFormular = React.createClass({
    mixins: [KnockoutMixin],
    getInitialState: function () {
        return {
            displayClass: 'hide',
            infoBoxContent: ''
        };
    },

    isValid: function () {
        var valid = Data.User.isValid();
        if (valid) {
            this.setState({ displayClass: 'hide' });
            return valid;
        }
        this.setState({
            displayClass: '',
            infoBoxContent: this.getInfoBoxContent(this.getFirstInValidElement())
        });
        return valid;
    },
    getInfoBoxContent: function (elementname) {
        switch (elementname) {
            case "username":
                return 'Der Nutzername darf nicht leer sein und muss aus Buchstaben und Zahlen bestehen.';
            case "firstname":
                return 'Der Vorname darf nicht leer sein und muss aus Buchstaben und Zahlen bestehen.';
            case "lastname":
                return 'Der Nachname darf nicht leer sein und muss aus Buchstaben und Zahlen bestehen.';
            case "password":
                return 'Das Passwort muss 6-12 Zeichen lang sein und muss einen Großbuchstaben, einen Kleinbuchstaben und eine Zahl enthalten.';
            case "rpassword":
                return 'Die beiden Passwörter stimmen nicht überein.';
            case "mail":
                return 'Dies scheint keine gültige Mailadresse zu sein.';
            default:
                return 'unknown error';
        }
    },
    getFirstInValidElement: function () {
        if (!Data.User.UserNameValid()) {
            return "username";
        }
        if (!Data.User.FirstNameValid()) {
            return "firstname";
        }
        if (!Data.User.LastNameValid()) {
            return "lastname";
        }
        if (!Data.User.PasswordValid()) {
            return "password";
        }
        if (!Data.User.RPasswordValid()) {
            return "rpassword";
        }
        if (!Data.User.MailValid()) {
            return "mail";
        }
    },

    render: function () {
        return (
            <div className="inst-outerdiv inst-userform">
                <h1 className="">Your first user</h1>
                <hr />
                <InfoBox id="infobox" className={this.state.displayClass} titletext="Bitte überprüfen Sie Ihre Eingaben." text={this.state.infoBoxContent} />
                <div className="inst-text">
                    <InputGroup type="text" title="User Name" placeholder="Maxi" value="Data.User.UserName" id="username" isValide={Data.User.UserNameValid} />
                    <InputGroup type="text" title="Firstname" placeholder="Max" value="Data.User.FirstName" id="firstname" isValide={Data.User.FirstNameValid} />
                    <InputGroup type="text" title="Lastname" placeholder="Mustermann" value="Data.User.LastName" id="lastname" isValide={Data.User.LastNameValid} />
                    <InputGroup type="password" title="Password" placeholder="password" value="Data.User.Password" id="password" isValide={Data.User.PasswordValid} />
                    <InputGroup type="password" title="Repeat Password" placeholder="password" value="Data.User.RPassword" id="rpassword" isValide={Data.User.RPasswordValid} />
                    <InputGroup type="text" title="Mail" placeholder="max.mustermann@monkee.com" value="Data.User.Mail" id="email" isValide={Data.User.MailValid} />
                </div>
            </div>
            );
    }
});


var DomainFormular = React.createClass({
    mixins: [KnockoutMixin],
    getInitialState: function () {
        return {
            displayClass: 'hide',
            infoBoxContent: ''
        };
    },

    isValid: function () {
        var valid = Data.Settings.isValid();
        if (valid) {
            this.setState({ displayClass: 'hide' });
            return valid;
        }
        this.setState({
            displayClass: '',
            infoBoxContent: this.getInfoBoxContent(this.getFirstInValidElement())
        });
        return valid;
    },
    getInfoBoxContent: function (elementname) {
        switch (elementname) {
            case "name":
                return 'Der Name der Webseite darf nicht leer sein und muss aus Buchstaben und Zahlen bestehen.';
            case "domain":
                return 'Url der Webseite nicht gültig.';
            default:
                return 'unknown error';
        }
    },
    getFirstInValidElement: function () {
        if (!Data.Settings.NameValid()) {
            return "name";
        }
        if (!Data.Settings.DomainValid()) {
            return "domain";
        }
    },

    render: function () {
        return (
            <div className="inst-outerdiv inst-domainform">
                <h1 className="">Your first page</h1>
                <hr />
                <div className="inst-text">
                    <InfoBox id="infobox" className={this.state.displayClass} titletext="Bitte überprüfen Sie Ihre Eingaben." text={this.state.infoBoxContent} />
                    <InputGroup type="text" title="Name" placeholder="name" value="Data.Settings.Name" id="name" isValide={Data.Settings.NameValid} />
                    <InputGroup type="text" title="Domain" placeholder="www.monkee.info" value="Data.Settings.Domain" id="domain" isValide={Data.Settings.DomainValid} />
                    <InputGroup type="text" title="Bindings" placeholder="www.monkee.info" value="DomainBinding" id="domainbinding" isValide={Data.Settings.DomainValid} readonly />
                </div>
            </div>
            );
    }
});
var GratulationContent = React.createClass({
    mixins: [KnockoutMixin],
    getInitialState: function () {
        return {
        };
    },
    render: function () {
        return (
            <div className="inst-outerdiv inst-gratulation">
                <h1>Congratulations, you have successfully installed your first page</h1>
                <div className="inst-text">
                    <p>
                        Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.
                    </p>
                </div>
                <a href="/admin/" class="btn btn-info pull-right" role="button">Zum Adminbreich</a>
            </div>
            );
    }
});
