import { ApiService, ApiType } from "../ApiService.js";
import { DefaultService } from "../DefaultService.js";
import { LocalStorageValues } from "../../Common/Enums.js";
import { Variable } from "../Variable.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";

export class LeftMenu {//handle left menu behaviours
    menuCallback = null; //callback when menu has been loaded
    json = null;

    constructor() {
        this._getData();
    }

    _getData() {//get menu items from server
        var getRoleId = localStorage.getItem("RoleId");
        var getUserId = localStorage.getItem("userToken");
        var data = {
            Id: getRoleId,
            UserId: getUserId
        }

        ApiService.call(ApiType.POST, variable.getMenu, data, (response) => {
            this._menu(response);
        });
    }

    _menu(data) {

        let menu = $(".sidebar");
        menu[0].innerHTML = "";

        let path = window.location.pathname;
        var menuURL = null;

        let grandParents = [];
        for (let grandParent of data) {
            let active = "";
            let parentActive = "";
            let grandparentActive = "";

            let parents = [];
            for (let parent of grandParent.subMenu) {
                let childs = [];
                for (let child of parent.child) {
                    
                    if (child.url != "" && path.toLowerCase().includes(child.url.toLowerCase())) {
                        menuURL = child.id;
                        active = "active";
                        parentActive = " active";
                        grandparentActive = " active";
                    } else {
                        active = "";
                    }
                    let ref = "/" + child.url;
                    let target = "_self";
                    if (child.url.toLowerCase().includes("loginbypass")) {
                        ref = variable.externalURL + child.url + localStorage.getItem(LocalStorageValues.EMAIL);
                        target = "_blank";
                    }
                    childs.push(DefaultService.createElement("li", { class: "CHILD-2 " }, [
                        DefaultService.createElement("a", { class: " menu-item " + active, href: ref, id: child.id, target:target }, [
                            DefaultService.createElement("i", { class: child.icon }, [], {}),
                            DefaultService.createElement("span", { class: "span-small ms-2" }, [], { innerHTML: child.name })
                        ]),
                    ]));
                }

                let childView = DefaultService.createElement("ul", { class: "sub-menu CHILD-1" }, childs)
                
                var arrowParent = "";
                var hrefParent = "/" + parent.url;
                if (childs.length > 0) {
                    arrowParent = "fa fa-chevron-right";
                    hrefParent = "javascript:void(0)";
                }
                else if (hrefParent != "/" && path.toLowerCase().includes(hrefParent.toLowerCase())) {
                    menuURL = parent.id;
                    active = "active";
                    grandparentActive = " active"
                } else {
                    active = "";
                }
                let target = "_self";
                if (parent.url.toLowerCase().includes("loginbypass")) {
                    hrefParent = variable.externalURL + parent.url + localStorage.getItem(LocalStorageValues.EMAIL);
                    target = "_blank";
                }
                parents.push(DefaultService.createElement("li", { class: "PARENT-2 " }, [
                    DefaultService.createElement("a", { class: " w-fit menu-item " + active + parentActive, href: hrefParent, id: parent.id, target: target }, [
                        DefaultService.createElement("i", { class: parent.icon}, [], {}),
                        DefaultService.createElement("span", { class: "span-small ms-2" }, [], { innerHTML: parent.name }),
                        DefaultService.createElement("i", { class: arrowParent + " float-right mt-1" }, [], {})
                    ], {}), childView
                ]));

                parentActive = ""
            }
            let parentView = DefaultService.createElement("ul", { class: "sub-menu PARENT-1" }, parents)

            var arrowGrandParent = "";
            var hrefGrandParent = "/" + grandParent.url;
            if (parents.length > 0) {
                arrowGrandParent = "fa fa-chevron-right";
                hrefGrandParent = "javascript:void(0)";
            } else if (hrefGrandParent != "/" && path.toLowerCase().includes(hrefGrandParent.toLowerCase())) {
                menuURL = grandParent.id;
                active = "active";
            }
            let target = "_self";
            if (grandParent.url.toLowerCase().includes("loginbypass")) {
                hrefGrandParent = variable.externalURL + grandParent.url + localStorage.getItem(LocalStorageValues.EMAIL);
                target = "_blank";
            }
            grandParents.push(DefaultService.createElement("li", { class: "GRANDPARENT-2 " }, [
                DefaultService.createElement("a", { class: " w-fit menu-item " + active + grandparentActive, href: hrefGrandParent, id: grandParent.id, target: target }, [
                    DefaultService.createElement("i", { class: grandParent.icon }, [], {}),
                    DefaultService.createElement("span", { class: "span-small ms-2" }, [], { innerHTML: grandParent.name  }),
                    DefaultService.createElement("i", { class: arrowGrandParent + " float-right mt-1" }, [], {  })
                ], {}), parentView
            ]));

           

        }
        menu.append(
            DefaultService.createElement("ul", { class: "GRANDPARENT-1" }, grandParents)
        );

        if (menuURL != null) {
            sessionStorage.setItem(LocalStorageValues.MODULEID, menuURL);
        }
        
        this._initSideMenuListener();

    }


    _initSideMenuListener() {
        $('.sidebar ul li a').click(function (ev) {

            $('.sidebar .sub-menu').not($(this).parents('.sub-menu')).slideUp();
            //const classNames = ["fa-chevron-right", "fa-chevron-down"];

            //if (classNames.some(className => $(this)[0].lastChild.classList.contains(className))) {
            //    console.log("here")
               
            //    $(this)[0].lastChild.classList.toggle("fa-chevron-right");
            //    $(this)[0].lastChild.classList.toggle("fa-chevron-down");

            //}
            if ($(this)[0].nextSibling != null || $(this)[0].nextSibling != undefined) {
                if ($(this)[0].nextSibling.style.display == "block") {
                    $(this)[0].nextSibling.style.display = "none"
                } else {
                    $(this).next('.sub-menu').slideToggle();
                }
            }

            ev.stopPropagation();
        });
    }
}

window.LeftMenu = new LeftMenu();

