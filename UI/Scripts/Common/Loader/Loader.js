export class Loader {//class to handle loading animation
    static showLoader(text = 'Loading..') {//show the animation
        $("#loader-custom-text").text(text)
        $("#loader-common").addClass("visible");
        $("#loader-common").removeClass("not-visible");
    }
    static hideLoader() {//hide the animation
        $("#loader-common").removeClass("visible");
        $("#loader-common").addClass("not-visible");
    }

}
window.Loader = new Loader();
