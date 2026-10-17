import { ApiService, ApiType } from "../Common/apiService.js";

export class DefaultService {


    static base64(file, callback) {//convert image to base64 => file -> JQuery input element, callback -> callback function
        var coolFile = {};
        function readerOnload(e) {
            var base64 = btoa(e.target.result);
            coolFile.base64 = base64;
            callback(coolFile)
        };

        var reader = new FileReader();
        reader.onload = readerOnload;

        var file = file[0].files[0];
        coolFile.filetype = file.type;
        coolFile.size = file.size;
        coolFile.filename = file.name;
        reader.readAsBinaryString(file);

    }

    static createElement(type, attributes, children = [], properties = {}) {
        var elem = document.createElement(type);
        for (var attribute in attributes) {
            elem.setAttribute(attribute, attributes[attribute]);
        }

        for (var i = 0; i < children.length; i++) {
            if (children[i] != null && typeof children[i] != "undefined") {
                if (typeof children[i]['classList'] != "undefined") {
                    elem.appendChild(children[i]);
                } else {
                    elem.appendChild(createElement(children[i]['type'], children[i]['attributes'], children[i]['children'] || [], children['properties'] || {}));
                }

            };
        }

        for (var property in properties) {
            if (property == "innerHTML") {
                elem[property] += properties[property];
            } else {
                elem[property] = properties[property];
            }
        }

        return elem;
    }

    static generate(length = 5) {
        var result = '';
        var characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
        var charactersLength = characters.length;
        for (var i = 0; i < length; i++) {
            result += characters.charAt(Math.floor(Math.random() * charactersLength));
        }
        return result;
    }

    static pad(number) {
        return number < 10 ? "0" + number : number;
    }


    /* --------------------------------- INPUT VALIDATION ------------------------------*/
    static validate(parent = null, scrollParent = null) {
        if (parent == null) { parent = document.body; }
        if (scrollParent == null) { scrollParent = parent; }
        const elems = $(parent).find('input, textarea, select');
        let ret = true;
        let password = '';
        this.clearValidateError();

        for (let i = 0; i < elems.length; i++) {
            const elem = elems[i];
            let validity;

            var special = /^[A-Za-z0-9, ]+$|^$/;

            //validating filled inputs
            if (
                elem.getAttribute("required") != null &&
                (!(validity = this.isInputValid(elem)).result || (elem['value'] === '' && elem.innerHTML === ''))) {

                ret = false;
                this.displayElementError(parent, scrollParent, elem, validity['msg']);
            }

            //validating wether the input contains special characters 
            else if (elem.getAttribute("special") != null && (!special.test(elem['value']) || !special.test(elem.innerHTML))) {
                this.displayElementError(parent, scrollParent, elem, "Special Characters are not allowed");
                ret = false;
            }

            if (elem.getAttribute("password") != null) {
                password = elem['value'] || elem.innerHTML;

                var passwordRegex = /^(?=.*?[!"#\$%&'\(\)\*\+,-\.\/:;<=>\?@[\]\^_`\{\|}~])/; //special char check
                if (!passwordRegex.test(password)) {
                    this.displayElementError(parent, scrollParent, elem, "Password should contain at least one special character");
                    ret = false;
                }
                passwordRegex = /^(?=.*[0-9])/;//number check
                if (!passwordRegex.test(password)) {
                    this.displayElementError(parent, scrollParent, elem, "Password should contain at least one number");
                    ret = false;
                }
                passwordRegex = /^(?=.*?[A-Z])/; //uppercase check
                if (!passwordRegex.test(password)) {
                    this.displayElementError(parent, scrollParent, elem, "Password should contain at least one uppercase");
                    ret = false;
                }
                passwordRegex = /^(?=.*?[a-z])/; //lowercase check
                if (!passwordRegex.test(password)) {
                    this.displayElementError(parent, scrollParent, elem, "Password should contain at least one lowercase");
                    ret = false;
                }
                var passwordRegex = /^[a-zA-Z0-9]{6,}/; //length check
                if (!passwordRegex.test(password)) {
                    this.displayElementError(parent, scrollParent, elem, "Password should be at least 6 characters long");
                    ret = false;
                }
            }

            if (elem.getAttribute("password-repeat") != null && password != (elem['value'] || elem.innerHTML)) {
                this.displayElementError(parent, scrollParent, elem, "Passwords do not match");
                ret = false;
            }
        }
        return ret;
    }

    static displayElementError(parent, scrollParent, elem, error ) {
        elem.classList.add("error");

        if (elem.previousElementSibling != null) {
            elem.previousElementSibling.classList.add("error");
            //display error message
            elem.previousElementSibling.setAttribute('errormsg', '  '+error);
        }
        $(scrollParent).animate({ scrollTop: $(elem).offset().top }, 100);
    }

    static result(parent = document.body, scrollParent = document.body) {
        if (this.validate(parent, scrollParent)) {
            var res = {};
            var chosen = [];
            var collections = $(parent).find(".collection");
            var groups = $(parent).find(".group");

            if (collections.length > 0) {
                for (var collection of collections) {
                    var innerResult = [];
                    $(collection).find(".group").each((k, elem) => {
                        var inner2 = {}
                        $(elem).find("input,textarea").each((j, innerElem) => {
                            inner2[innerElem.getAttribute("result")] = (innerElem).value || innerElem.innerHTML;
                            chosen.push(innerElem);
                        });
                        innerResult.push(inner2);
                    });
                    res[collection.getAttribute("result")] = innerResult;
                }
            } else if (groups.length > 0) {
                for (var group of groups) {
                    var name = group.getAttribute('result');
                    res[name] = {};
                    $(group).find("input,textarea").each((i, elem) => {
                        res[name][elem.getAttribute('result')] = elem.innerHTML || (elem).value;
                        chosen.push(elem);
                    })
                }
            }

            $(parent).find("input,textarea,select").each((i, elem) => {
                if (elem.getAttribute("result") != null && chosen.indexOf(elem) == -1) {
                    var type = elem['type'];
                    if (type == "number" || type == "text" || type == "phone" || type == "email" || type == "textarea" || type == "password" || type == "date" || type == "tel" || type == "time" || type == "datetime-local" || type == "color") {
                        res[elem.getAttribute("result")] = elem['value'];
                    } else if (type == "radio") {
                        if (elem['checked']) res[elem.getAttribute("result")] = elem.getAttribute("value");
                    } else if (type == "checkbox") {
                        if (elem['checked']) res[elem.getAttribute("result")] = elem.checked;
                    } else if (type == "select-one" || type == "select") {
                        if (elem['options'][elem['selectedIndex']] != undefined) {
                            let val = elem['options'][elem['selectedIndex']].value;
                            if (val == 0) {
                                res[elem.getAttribute("result")] = 0;
                            } else {
                                res[elem.getAttribute("result")] = parseInt(val);
                            }
                        } else {
                            res[elem.getAttribute("result")] = 0
                        }
                    } else if (type == "select-multiple") {
                        res[elem.getAttribute("result")] = [...elem.options].filter(y => y.selected == true).map(y => parseInt(y.value));
                    }
                }

            });
            return res;
        } else {
            return false;
        }
    }


    static resultObject(objectName ,parent = document.body, scrollParent = document.body) {
        if (this.validate(parent, scrollParent)) {
            var res = {};
            var chosen = [];
            var collections = $(parent).find(".collection");
            var groups = $(parent).find(".group");

            if (collections.length > 0) {
                for (var collection of collections) {
                    var innerResult = [];
                    $(collection).find(".group").each((k, elem) => {
                        var inner2 = {}
                        $(elem).find("input,textarea").each((j, innerElem) => {
                            inner2[innerElem.getAttribute(objectName)] = (innerElem).value || innerElem.innerHTML;
                            chosen.push(innerElem);
                        });
                        innerResult.push(inner2);
                    });
                    res[collection.getAttribute(objectName)] = innerResult;
                }
            } else if (groups.length > 0) {
                for (var group of groups) {
                    var name = group.getAttribute(objectName);
                    res[name] = {};
                    $(group).find("input,textarea").each((i, elem) => {
                        res[name][elem.getAttribute(objectName)] = elem.innerHTML || (elem).value;
                        chosen.push(elem);
                    })
                }
            }

            $(parent).find("input,textarea,select").each((i, elem) => {
                if (elem.getAttribute(objectName) != null && chosen.indexOf(elem) == -1) {
                    var type = elem['type'];
                    if (type == "text" || type == "phone" || type == "email" || type == "textarea" || type == "password" || type == "date" || type == "tel" || type == "time" || type == "datetime-local" || type == "color") {
                        res[elem.getAttribute(objectName)] = elem['value'];
                        if (elem.getAttribute(objectName) == "DOB") {
                            if (elem['value'] == "") {
                                res[elem.getAttribute(objectName)] = null;
                            }
                        }
                    } else if (type == "number") {
                        res[elem.getAttribute(objectName)] = elem['value'] == "" ? 0 : parseFloat(elem['value']);
                    } else if (type == "radio") {
                        if (elem['checked']) res[elem.getAttribute(objectName)] = elem.getAttribute("value");
                    } else if (type == "checkbox") {
                        if (elem['checked']) res[elem.getAttribute(objectName)] = elem.checked;
                    } else if (type == "select-one" || type == "select") {
                        if (elem['options'][elem['selectedIndex']] != undefined) {
                            let val = elem['options'][elem['selectedIndex']].value;
                            if (val == 0) {
                                res[elem.getAttribute(objectName)] = 0;
                            } else {
                                res[elem.getAttribute(objectName)] = parseInt(val);
                            }
                        } else {
                            res[elem.getAttribute(objectName)] = 0
                        }
                    }
                }

            });
            return res;
        } else {
            return false;
        }
    }

    static isInputValid(input) {
        var type = input.type;
        var value = input.innerHTML || input.value;
        var ret = true;

        var msg = "Mandatory field missing";//default message for required input
        switch (type) {
            case "email":
                if (!value.includes("@") || !value.includes(".")) {
                    ret = false
                    msg = "Invalid Email Format"
                }
                break;
            case "number":
                if (isNaN(parseInt(value))) {
                    ret = false;
                    msg = "Invalid Number";
                }
                break;
            case "phone":
                if (isNaN(parseInt(value)) || value > 30 || value < 4) {
                    ret = false;
                    msg = "Invalid Phone Number | Do not add brackets";
                }
                break;
            case "radio":
                if (typeof validityRadio[input.name]) {
                    validityRadio[input.name] = false;
                }
                if (input.checked) validityRadio[input.name] = true;
                break;
            case "select-one":
                if (typeof input['options'][input['selectedIndex']] == "undefined") {
                    ret = false;
                    msg = "Mandatory field missing";
                }
        }

        if (input.getAttribute("minlength") != null && value.length < input.getAttribute("minlength")) {
            ret = false;
            msg = " Value is too Short (> " + input.getAttribute("minlength") + ")";
        }
        if (input.getAttribute("maxlength") != null && value.length > input.getAttribute("maxlength")) {
            ret = false;
            msg = " Value is too Long (< " + input.getAttribute("maxlength") + ")";
        }
        return { result: ret, msg: msg };
    }

    static clearValidateError() {
        //var errorElems = document.getElementsByClassName("error");
        //while (errorElems.length > 0) {
        //    var errorElem = errorElems[0];
        //    (errorElem.previousSibling).style.marginBottom = "auto";
        //    removeElement(errorElem);
        //}

        $(".error").removeClass("error");
    }

    static insertAfter(newNode, referenceNode) {
        referenceNode.parentNode.insertBefore(newNode, referenceNode.nextSibling);
    }
    /* --------------------------------- END INPUT VALIDATION ------------------------------*/

    static removeElement(element) {//remove element from DOM completely... takes JAVSCRIPT element as param not JQUERY
        element.parentNode.removeChild(element);
    }

}
