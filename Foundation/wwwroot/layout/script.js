function toggleMenu(id) {
    var body = document.getElementById("body");
    //alert(id + "-opened");
    body.classList.toggle(id+"-opened");
    body.classList.toggle(id + "-closed");
}
