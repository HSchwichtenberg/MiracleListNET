function setSwitchValue(id, value) {

 var action = "noset";
 if (value === true) action = "on";
 if (value === false) action = "off";
 //console.log("setSwitchValue", id, value, action);
 $("#" + id).setTheSwitchValue(action);
}

function initSwitch(id, value, tristate, caller) {
// bool? in on/off/noset umsetzen
 var action = "noset";
 if (value === true) action = "on";
 if (value === false) action = "off";
 //console.log("initTriState", id, value, tristate, caller, action);
 $("#" + id).setTheSwitch({
  action: action,
  steps: (tristate ? 3 : 2 ),
  bgOn: '#BCF5A9',
  bgNoSet: '#f1f1f1',
  bgOff: '#F6CED8',
  width: 60,
  porcent: false,
  height: 16,
  onLabel: '●',
  offLabel: '●',
  hsize: 11,
  onSet: function (e) {
   $("#" + id).getTheSwitchValue(value => {
    //$("#value-" + id).html(value);
   });
  },
  onClickOn: function (e) {
   var element = $(e).attr('id');
   //console.log("client basic click on " + element);
   $("#" + id).getTheSwitchValue(value => {
    //$("#value-" + id).html(value);
    caller.invokeMethodAsync("SetValue", true);
   });

  },
  onClickNoSet: function (e) {
   var element = $(e).attr('id');
   //console.log("client basic click noset " + element);
   $("#" + id).getTheSwitchValue(value => {
    //$("#value-" + id).html(value);
    caller.invokeMethodAsync("SetValue", null);
   });

  },
  onClickOff: function (e) {
   var element = $(e).attr('id');
   //console.log("client basic click off " + element);
   $("#" + id).getTheSwitchValue(value => {
    $("#value-" + id).html(value);
    caller.invokeMethodAsync("SetValue", false);
   });

  }
 });
}


console.log("Loaded " + document.currentScript.src + "!");