
import { DefaultService } from "/Scripts/Common/DefaultService.js";
export class SectionRendering {//class to handle loading animation

    static sectionRenderer(section = []) {
        let sections = [];
        if (section.length > 0) {
            for (var [i, se] of section.entries()) {
                var active = "";
                var color = "";
                if (i == 0) {
                    active = "active-section";
                    color = "naveo-color"
                }

                var icon = "";
                switch (se.sectionStatus) {
                    case 1:
                        icon = DefaultService.createElement("i", { class: "fa-regular fa-circle me-2" }, []);
                        break;

                    case 2:
                        icon = DefaultService.createElement("i", { class: "fa-regular fa-circle-dot me-2" }, []);
                        break;

                    case 3:
                        icon = DefaultService.createElement("i", { class: "fa fa-circle-check me-2" }, []);
                        break;

                    default:
                        icon = DefaultService.createElement("i", { class: "fa-regular fa-circle-dot me-2" }, []);
                        break;
                }

                var title = DefaultService.createElement("div", { class: "section-menu " + active, id: se.sectionCode, onclick: "Template.changeView('" + se.sectionCode + "')" }, [
                    DefaultService.createElement("span", { class: "span-medium" }, [icon], { innerHTML: se.sectionName }),
                ]);

                sections.push(title);
            }
        } else {
            var title = DefaultService.createElement("div", { class: "section-menu " + active, id: se.sectionCode }, [
                DefaultService.createElement("span", { class: "span-medium" }, [
                    DefaultService.createElement("i", { class: "fa-regular fa-circle-dot me-2" }, [])
                ], { innerHTML: "Scheme" }),
            ]);

            sections.push(title);
        }
        
        $(".section-container").html(sections);
    }


    static scrollspyRenderer(virtualDatum) {
        var hiddenClass = "";
        if (virtualDatum.hidden) {
            hiddenClass = " d-none"
        }
        let scrollspy = DefaultService.createElement("div", { class: "m-3" + hiddenClass }, [
            DefaultService.createElement("a", { href: "#scrollspy-" + virtualDatum.name.replace(/ /g, '_') }, [
                DefaultService.createElement("span", { class: "span-small" }, [], { innerHTML: virtualDatum.name })
            ], {}),
        ]);

        return scrollspy;
       
    }

    static contentPanelRenderer(virtualData, elem) {
        var hiddenClass = "";
        if (virtualData.hidden) {
            hiddenClass = " d-none"
        }

        //main conatenet 
        let content = DefaultService.createElement("div", { id: "scrollspy-" + virtualData.name.replace(/ /g, '_'), class: "scrollspy-transition" + hiddenClass }, [
            //section
            DefaultService.createElement("div", { class: "section", 'data-bs-toggle': "collapse", href: "#" + virtualData.name.replace(/ /g, '_'), 'aria-expanded': "true" }, [
                DefaultService.createElement("span", { class: "span-medium" }, [], { innerHTML: virtualData.name }),
                DefaultService.createElement("span", { class: "collapsible-icon" }, [], {}),

            ]),
            DefaultService.createElement("div", { id: virtualData.name.replace(/ /g, '_'), class: "collapse form-panel show" }, elem)
        ]);
        return content;
    }


}
window.SectionRendering = new SectionRendering();
