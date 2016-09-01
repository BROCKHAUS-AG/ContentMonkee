
var Input = React.createClass({
    getInitialState: function () {
        return {
        };
    },
    render: function () {
        return (
            <input className={this.props.className} id={this.props.id} data-bind={"value:"+this.props.text} />
            );
    }
});
var Span = React.createClass({
    getInitialState: function () {
        return {
        };
    },
    render: function () {
        return (
            <span className={this.props.className} id={this.props.id} data-bind={"text:'"+this.props.text+"'"}></span>
            );
    }
});

var Image = React.createClass({
    getInitialState: function () {
        return {

        };
    },
    render: function () {
        return (
            <img className={this.props.className} src={this.props.src} alt={this.props.alt} id={this.props.id} />
            );
    }
});

var Button = React.createClass({
    clickHandler: function (e) {
        if (typeof this.props.onClick === 'function') {
            this.props.onClick(this);
        }
    },
    getInitialState: function () {
        return {
        };
    },
    render: function () {
        return (
        <button className={this.props.className} id={this.props.id} onClick={this.clickHandler} disabled={this.props.disabled}>{this.props.text}</button>
        );
    }
});


var InputGroup = React.createClass({
    getInitialState: function () {
        return {
            valideClass: '',
        };
    },

    updateValideClass: function (e) {
        var isValide = this.props.isValide();
        if (isValide && this.state.valideClass == '' ||
            !isValide && this.state.valideClass != '') {
            return false;
        }
        if (isValide) {
            this.setState({ valideClass: '' });
        } else {
            this.setState({ valideClass: 'fa-times' });
        }
        return true;
    },
    resetColorClass: function () {
        //this.setState({ valideClass: '' });
    },
    render: function () {
        return (
            <div className="form-group">
                <label htmlFor={this.props.id} className="col-sm-3 control-label">{this.props.title}</label>                
                <div className="col-sm-8">                       
                    <input className={"form-control"}
                           id={this.props.id}
                           type={this.props.type}
                           data-bind={"value:" + this.props.value+", valueUpdate:'keydown'"}
                           placeholder={this.props.placeholder}
                           onBlur={this.updateValideClass}
                           onClick={this.resetColorClass} />
                </div>
                <i className={"fa " + this.state.valideClass + " col-sm-1"} aria-hidden="true"></i> 
            </div>
        );
    }
});
var InfoBox = React.createClass({
    getInitialState: function () {
        return {
        };
    },
    render: function () {
        return (
            <div className={"infobox "+this.props.className}>
                <h2>{this.props.titletext}</h2>
                <hr />
                <Span id={this.props.id} text={this.props.text}></Span>
            </div>
            );
    }
});