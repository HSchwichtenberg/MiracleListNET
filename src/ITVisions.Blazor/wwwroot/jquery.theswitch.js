var g_o = [];
var g_o_i = 0;
(function($){
    $.fn.setTheSwitch = function(options) {
        defaults = {
            steps: 3,
            bgOn: '#4da154',
            bgNoSet: '#f1f1f1',
            bgOff: '#d82c2c',
            width: 60,
            porcent: false,
            height: 14, 
            action: 'noset',
            onLabel: '●',
            offLabel: '●',
            hsize: 11,
            disabled: false,
            onSet: function(e) {},
            onClickOn: function(e) {},
            onClickNoSet: function(e) {},
            onClickOff: function(e) {}
        };
        var dataValue = $(this).attr("data-value");
        if(typeof dataValue === 'undefined') {
            var options = $.extend({}, defaults, options);
            $(this).attr('data-option', g_o_i++ );
            g_o.push(options);
            createSwitchComponent(this, options);
        }
    }

    function createSwitchComponent(_root, options) {
        $(_root).html('');

        var sizeType = "px";
        if(options.height < 4 ) options.height = 4;
        if(options.porcent==true) sizeType = "%"; 
        var width = (options.width+6).toString() + sizeType;
        if(options.steps!=2&&options.steps!=3) {
            options.steps = 3;
        }
        if(options.steps==2 && options.action=="noset") {
            options.action = "off";
        }
        var widthPart = (options.width / options.steps);
        var height = options.height.toString() + sizeType;
        var marginHeigh = options.height * 0.1;
        var marginHe = marginHeigh.toString() + "px";
        var borderColor = "#bdbdbd";
        var radius = (options.height/2).toString() + "px"
        
        var elementName = $(_root).attr("id"); 
        var elementRailOn = elementName + "_railon";
        var elementRailNoSet = elementName + "_railnoset";
        var elementRailOff = elementName + "_railoff";

        var railOffCss = {
            "cursor": "pointer",
            "color": "#ff0000",
            "font-size": "0.7em",
            "text-align": "center",
            "display": "inline-block", 
            "width": widthPart.toString() + sizeType, 
            "vertical-align": "top",
            "border-left": "1px solid " + borderColor, 
            "border-top": "1px solid " + borderColor, 
            "border-bottom": "1px solid " + borderColor, 
            "border-top-left-radius": radius, 
            "border-bottom-left-radius": radius, 
            "height": "100%",
            "margin-top": marginHe,
            "margin-bottom": marginHe
        };
        var railNoSetCss = {
            "cursor": "pointer",
            "display": "inline-block", 
            "text-align": "center",
            "width": widthPart.toString() + sizeType, 
            "vertical-align": "top",
            "border-top": "1px solid " + borderColor, 
            "border-bottom": "1px solid " + borderColor, 
            "height": "100%",
            "margin-top": marginHe,
            "margin-bottom": marginHe
        };
        var railOnCss = {
            "cursor": "pointer",
            "color": "#089710",
            "font-size": "0.7em",
            "text-align": "center",
            "display": "inline-block", 
            "width": widthPart.toString() + sizeType, 
            "vertical-align": "top",
            "border-right": "1px solid " + borderColor, 
            "border-top": "1px solid " + borderColor, 
            "border-bottom": "1px solid " + borderColor, 
            "border-top-right-radius": radius, 
            "border-bottom-right-radius": radius, 
            "height": "100%",
            "margin-top": marginHe,
            "margin-bottom": marginHe
        };

        $(_root).css({ "display": "inline-block", "width": width, "height": height });
        var switchRailOn = $("<div>&nbsp;</div>").css(railOnCss).attr("id", elementRailOn);
        var switchRailNoSet = $("<div>&nbsp;</div>").css(railNoSetCss).attr("id", elementRailNoSet);
        var switchRailOff = $("<div>&nbsp;</div>").css(railOffCss).attr("id", elementRailOff);
        
        setTheSwitchHandler(_root, options.action,  switchRailOn, switchRailNoSet, switchRailOff, options, action=>{
            options.onSet(_root);
        });
    }

    function setTheSwitchHandler(element, action,  switchRailOn, switchRailNoSet, switchRailOff, options, callBack) {

        setOptionsAction(element, action, true);

        var radius = (14/2).toString() + "px";

        $(switchRailOn).html('');
        $(switchRailNoSet).html('');
        $(switchRailOff).html('');
        
        $(element).append(switchRailOff);
        if(options.steps==3) {
            $(element).append(switchRailNoSet)
        }
        $(element).append(switchRailOn);

        var handlerCss = {
            "cursor": "pointer",
            "display": "inline-block",
            "height": options.hsize.toString() + "px", 
            "width": options.hsize.toString() + "px", 
            "border-radius": radius,
            "border": "1px solid #d5e1ee",
            "background-image": "linear-gradient(bottom, rgb(234,241,249) 0%, rgb(234,241,249) 90%)",
            "-moz-box-shadow": "1px 2px 4px #5d5d5d",
            "-webkit-box-shadow": "1px 2px 4px #5d5d5d",
            "box-shadow": "1px 2px 4px #5d5d5d"
        };

        if(options.disabled) { 

            handlerCss.display = "none";

        }


        $(element).attr("data-value", action);

        var handler = $("<div draggable='true'>&nbsp;</div>");

        var onLabel = "<span>" + (!options.disabled ? options.onLabel : '') + "<span>";
        var offLabel = "<span>" + (!options.disabled ? options.offLabel : '') + "<span>";

        switch(action) {
            case 'on':
                handlerCss.float = 'right';
                if(options.steps == 2) {
                    $(offLabel).css({ "float": "right"});
                }
                $(handler).css(handlerCss);
                
                $(switchRailNoSet).css({ "background-color": options.bgOn });
                $(switchRailOn).append(handler).css({ "background-color": options.bgOn });
                $(switchRailOff).append(offLabel).css({ "background-color": options.bgOn });
                
                $(switchRailOn).css({ "background-color": options.bgOn });
                $(switchRailNoSet).css({ "background-color": options.bgOn });
                $(switchRailOff).css({ "background-color": options.bgOn });;
            break;
            case 'noset':
                handlerCss.float = 'center';
                $(handler).css(handlerCss);
                $(switchRailNoSet).append(handler);
                $(switchRailOn).append(onLabel);
                $(switchRailOff).append(offLabel);

                $(switchRailOn).css({ "background-color": options.bgNoSet });
                $(switchRailNoSet).css({ "background-color": options.bgNoSet });
                $(switchRailOff).css({ "background-color": options.bgNoSet });;
            break;
            case 'off':
                handlerCss.float = 'left';
                if(options.steps == 2) {
                    $(onLabel).css({ "float": "left"});
                }
                $(handler).css(handlerCss);
                $(switchRailOn).append(onLabel);
                $(switchRailOff).append(handler);

                $(switchRailOn).css({ "background-color": options.bgOff });
                $(switchRailNoSet).css({ "background-color": options.bgOff });
                $(switchRailOff).css({ "background-color": options.bgOff });;
            break;
        }

        if(!options.disabled) { 

            $(switchRailOn).off( "click" );
            $(switchRailOn).click(e=>{
                if(action!='on') {
                    setTheSwitchHandler(element, 'on',  switchRailOn, switchRailNoSet, switchRailOff, options, function(action){  });       
                    options.onClickOn(element);
                }
            });
            if(options.steps==3) {
                $(switchRailNoSet).off( "click" );
                $(switchRailNoSet).click(e=>{
                    if(action!='noset') {
                        setTheSwitchHandler(element, 'noset',  switchRailOn, switchRailNoSet, switchRailOff, options, function(action){ });
                        options.onClickNoSet(element);
                    }
                });
            }
            $(switchRailOff).off( "click" );
            $(switchRailOff).click(e=>{
                if(action!='off') {
                    setTheSwitchHandler(element, 'off',  switchRailOn, switchRailNoSet, switchRailOff, options, function(action){ });
                    options.onClickOff(element);
                }
            });
            
        }
            
        callBack(action);

    }

    $.fn.getTheSwitchValue = function(sendData) {
        var value =  $(this).attr("data-value");
        sendData(value);
    }

    $.fn.setTheSwitchValue = function(action) {
        setOptionsAction(this, action);
    }

    $.fn.enabledTheSwitch = function(enabled) {
        enabledSwitch(this, enabled);
    }

    function setOptionsAction(_root, action, nodraw) {
        var index = parseInt($(_root).attr('data-option'));
        g_o[index].action = action;
        if( typeof nodraw === 'undefined' ) {
            createSwitchComponent(_root, g_o[index]);
        }      
        else if(!nodraw) {
            createSwitchComponent(_root, g_o[index]);
        }          
    }

    function enabledSwitch(_root, enabled, nodraw) {
        var index = parseInt($(_root).attr('data-option'));
        g_o[index].disabled = !enabled;
        if( typeof nodraw === 'undefined' ) {
            createSwitchComponent(_root, g_o[index]);
        }      
        else if(!nodraw) {
            createSwitchComponent(_root, g_o[index]);
        }          
    }

}(jQuery))
