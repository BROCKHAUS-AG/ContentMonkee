

var App = React.createClass({
    mixins: [KnockoutMixin],
    getInitialState: function () {
        return {
            CurrentItemId: 0,
            ContentItems: [
                <WelcomeContent ref="contentpanel" />,
                <InstructionContent ref="contentpanel" />,
                <UserFormular ref="contentpanel" />,
                <DomainFormular ref="contentpanel" />,
                <GratulationContent ref="contentpanel" />
            ]
        };
    },

    nextState: function (sender) {
        if (typeof this.refs.contentpanel.isValid === 'function' && !this.refs.contentpanel.isValid()) {
            return;
        }
        this.setState({ CurrentItemId: Math.min(this.state.ContentItems.length - 1, this.state.CurrentItemId + 1) });
        if (this.state.CurrentItemId >= 3) {
            InstallData();
        }
    },

    prevState: function (sender) {
        this.setState({ CurrentItemId: Math.max(0, this.state.CurrentItemId - 1) });
    },

    getNextButton: function () {
        if (this.state.CurrentItemId < this.state.ContentItems.length - 1) {
            return (
                <Button className="control-button btn-default" onClick={this.prevState} text="Zurück" />
            );
        }
    },
    getPrevButton: function () {
        if (this.state.CurrentItemId < this.state.ContentItems.length - 1) {
            return (
                <Button className="control-button btn-default pull-right" onClick={this.nextState} text="Weiter" />
            );
        }
    },
    GetVisibleClassBtnNext: function () {
        return this.state.CurrentItemId < this.state.ContentItems.length - 1 ? "show" : "hide";
    },
    GetVisibleClassBtnPrev: function () {
        return this.state.CurrentItemId > 0 ? "show" : "hide";
    },

    render: function () {
        return (
            <div className="container">
                <Image className="inst-image" src="/App_Themes/admin/img/contentmonkee_logo_Affe_web.png" alt="monkee" id="monkee_logo" />

                <div className="panel panel-default">
                    <div className="panel-heading">Installing ContentMonkee step {this.state.CurrentItemId+1} of {this.state.ContentItems.length}</div>
                    <div className="panel-body">{this.state.ContentItems[this.state.CurrentItemId]}</div>
                    <div className="panel-footer">
                        <div className="row">
                            <div className="col-sm-2">
                                <Button ref="prevButton" className={"control-button btn btn-default " + this.GetVisibleClassBtnPrev()} onClick={this.prevState} text="Zurück" />
                            </div>
                            <div className="col-sm-8"></div>
                            <div className="col-sm-2">
                                <Button ref="nextButton" className={"control-button btn btn-default pull-right " + this.GetVisibleClassBtnNext()} onClick={this.nextState} text="Weiter" />
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        );
    }
});


ReactDOM.render(
  <App />,
  document.getElementById('content')
);