// ITVisions.Blazor 
// JavaScript Utilities  for Blazor
// (C) Dr. Holger Schwichtenberg 2019-2022

 console.log("Loading: ITV.BlazorUtil");

 window.log = (s) => {
  console.log(s);
 };

 window.ShowAlert = (text) => {
  console.log("ShowAlert", text);
  alert(text);
 };

 // DEMO: 21. JS-Interop einfach (JS)
 function ShowConfirm(text1, text2) {
  console.log("ShowConfirm", text1, text2);
  return confirm(text1 + "\n" + text2);
 }

 function SetTitle(text) {
  console.log("SetTitle", text);
  window.document.title = text;
 }

 window.Util = {
  //focusElement: function (element) {
  // element.focus();
  //},

  getBrowserInfo: function () {
   var data = [];
   data.push("Browser Type: " + navigator.appCodeName);
   data.push("Browser Version: " + navigator.appVersion);
   return data;
  },

  //https://www.xspdf.com/help/50910264.html
  getBrowserShortInfo: function () {
   var ua = navigator.userAgent, tem,
    M = ua.match(/(opera|chrome|safari|firefox|msie|trident(?=\/))\/?\s*(\d+)/i) || [];
   if (/trident/i.test(M[1])) {
    tem = /\brv[ :]+(\d+)/g.exec(ua) || [];
    return 'IE ' + (tem[1] || '');
   }
   if (M[1] === 'Chrome') {
    tem = ua.match(/\b(OPR|Edge)\/(\d+)/);
    if (tem != null) return tem.slice(1).join(' ').replace('OPR', 'Opera');
   }
   M = M[2] ? [M[1], M[2]] : [navigator.appName, navigator.appVersion, '-?'];
   if ((tem = ua.match(/version\/(\d+)/i)) != null) M.splice(1, 1, tem[1]);
   return M.join(' ');
  }

 }



 var screenInfoID = "screenInfo";

 window.initUpdateScreenSize = (id) => {
  //console.warn("ITVBlazor:initDisplayScreenSize");
  window.addEventListener('resize', window.UpdateScreenSize);
  window.UpdateScreenSize();
 };

 window.UpdateScreenSize = () => {
  var e = document.getElementById(screenInfoID);
  //console.warn("ITVBlazor:UpdateScreenSize", e);
  if (!e) return;
  e.textContent = getScreenSize();
 };

 // liefert die Bildschirmgröße
 window.getScreenSize = () => {
  return document.querySelector('html').clientWidth + "x" + document.querySelector('html').clientHeight + " (" + getScreenSizeName(document.querySelector('html').clientWidth) + ")";
 };

 // Berechnet die Bootstrap-Screen-Größe anhand der Breite
 function getScreenSizeName(width) {
  let sizeName = "";
  switch (true) {
   case (width >= 1200): sizeName = "xl"; break;
   case (width >= 992): sizeName = "lg"; break;
   case (width >= 768): sizeName = "md"; break;
   case (width >= 576): sizeName = "sm"; break;
   default: sizeName = "xs"; break;
  }
  return sizeName;
 }




 function IsHttps() {
  return (document.location.protocol == 'https:');
 }

 window.setCookie = (cookie) => {
  document.cookie = cookie;
  return true;
 };

 window.getCookie = () => {
  return document.cookie;
 };

 // #region Notifications

 // Rechte für Desktop Notifications anfordern beim Benutzer
 window.requestPermission = (head, text) => {
  console.log("requestPermission", head, text);

  Notification.requestPermission().then(function (result) {
   console.log("requestPermission Result", result);
   showNotification(head, text);
  });
 };
 // Desktop Notification zeigen
 window.showNotification = (head, text) => {
  console.log("showNotification", head, text);
  var img = '/_content/ITVisions.Blazor/ITVisions.jpg';
  var notification = new Notification(head, { body: text, icon: img });
  return true;
 };

 // #end region

 //window.openDropDown = function (element) {
 // console.log("openDropDown", element);
 // var event;
 // event = document.createEvent('MouseEvents');
 // event.initMouseEvent('mousedown', true, true, window);
 // element.dispatchEvent(event);
 //};

 window.toggleDropdown = function (element) {
  console.log("toggleDropDown", element);
  if (element.size == 0) {
   element.size = element.length;
   element.focus();
  }
  else element.size = 0;
 }

 console.log("Loaded: ITV.BlazorUtil");