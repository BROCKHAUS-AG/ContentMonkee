var ScrollToTop = function(pOffset, pText)
{
	offset = pOffset;
	text   = (pText   != null ? pText   : "Zur&uuml;ck zum Anfang")
	style  = "display:none; position: fixed; bottom: 60px; right: 0px; padding: 10px 15px; float: right; z-index: 100; background: #777; color: #FFF; opacity: 0.8; text-align: center; cursor: pointer;";

	this.create = function()
	{
		this.calcOffset();
		//$('body').append('<div id="scrollBox" style="' + style + '">' + text + '</div>');
		element = $('#headerlogo');
		element.click(this.scrollUp);
		//$(window).scroll(this.updateScrollBox);
		//this.updateScrollBox();
	}

	this.calcOffset = function()
	{
		if(offset > 100 || offset < 0 || offset == null) { offset = 50; }
		else if(offset == 0) { offset = 0; }
		else 
		{  
			var height = window.innerHeight;
			offset = (height / (100 / offset));
		}
	}
	
	this.scrollUp = function()
	{
	    infinitecontent.block();
		$('html, body').animate({ scrollTop: 0 }, 1000).promise().done(function(){
		    infinitecontent.unblock();
        });
	}

	this.updateScrollBox = function()
	{
		if($(window).scrollTop() >= offset) { element.fadeIn(); }
		else { element.fadeOut(); }
	}
	
	this.create();
}