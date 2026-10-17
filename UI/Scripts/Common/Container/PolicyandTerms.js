import { DefaultService } from "../DefaultService.js";
export class policyAndTerms {

    constructor() {
    }

    //privacy policy popup
    privacyPolicy() {
        $(".contentCompanyInfo").html("");
        let policyContent = DefaultService.createElement("div", {}, [
            //DefaultService.createElement("img", { class: "popupTopIcon" }, [], { id: "headerLogo", src: "/asset/image/Common/Logo/ebmslogo.png" }),
            DefaultService.createElement("div", { class: "popupContent m-4" }, [
                /*DefaultService.createElement("p", { class: "f-weight-bold" }, [], { innerHTML: "1	Effective Date: June 08, 2018" }),*/
                DefaultService.createElement("img", { class: "loginLogo" }, [], { src: "/assets/Common/Logo/logo-naveo-tag.png" }),
                DefaultService.createElement("h3", { class: "f-weight-bold" }, [], { innerHTML: "Privacy Policy" }),

                DefaultService.createElement("p", {}, [], {
                    innerHTML: 'At Naveo, accessible from https://www.naveo.mu, one of our main priorities is the privacy of our visitors. This Privacy Policy document contains types of information that is collected and recorded by Naveo and how we use it.'
                        + 'If you have additional questions or require more information about our Privacy Policy, do not hesitate to contact us.This Privacy Policy applies only to our online activities and is valid for visitors to our website with regards to the information that they shared and/ or collect in Naveo.'
                        + 'This policy is not applicable to any information collected offline or via channels other than this website.'
                }),

                DefaultService.createElement("p", { class: "f-weight-bold" }, [], { innerHTML: "Consent" }),
                DefaultService.createElement("p", {}, [], { innerHTML: 'By using our website, you hereby consent to our Privacy Policy and agree to its terms.' }),


                DefaultService.createElement("p", { class: "f-weight-bold" }, [], { innerHTML: "Information we collect" }),
                DefaultService.createElement("p", {}, [], {
                    innerHTML: 'Generally speaking, the Company collects Personal Information related to your use of Naveo. Specifically, we may collect the following Personal Information:'
                        + 'The personal information that you are asked to provide, and the reasons why you are asked to provide it, will be made clear to you at the point we ask you to provide your personal information.'
                        + ' If you contact us directly, we may receive additional information about you such as your name, email address, phone number, the contents of the message and/or attachments you may send us, and any other information you may choose to provide.'
                        + 'When you register for an Account, we may ask for your contact information, including items such as name, company name, address, email address, and telephone number.'
                }),


                DefaultService.createElement("p", { class: "f-weight-bold" }, [], { innerHTML: "How we use your information" }),
                DefaultService.createElement("p", {}, [], {
                    innerHTML: 'We use the information we collect in various ways, including to:'
                        + ' (a) Provide, operate, and maintain our website.'
                        + ' (b) Improve, personalize, and expand our website'
                        + ' (b) Understand and analyze how you use our webiste'
                        + ' (b) Develop new products, services, features, and functionality'
                        + ' Communicate with you, either directly or through one of our partners, including for customer service, to provide you with updates and other information relating to the webste, and for marketing and promotional purposes'
                        + 'Send you emails'
                        + 'Find and prevent fraud'

                }),
                DefaultService.createElement("p", { class: "f-weight-bold" }, [], { innerHTML: "Log Files" }),
                DefaultService.createElement("p", {}, [], {
                    innerHTML: 'Naveo follows a standard procedure of using log files. These files log visitors when they visit websites. All hosting companies do this and a part of hosting services` analytics.The information collected by log files include internet protocol(IP) addresses, browser type, Internet Service Provider(ISP), date and time stamp, referring/ exit pages, and possibly the number of clicks.These are not linked to any information that is personally identifiable.The purpose of the information is for analyzing trends, administering the site, tracking users` movement on the website, and gathering demographic information. Our Privacy Policy was created with the help of the Privacy Policy Generator.'

                }),


                DefaultService.createElement("p", { class: "f-weight-bold" }, [], { innerHTML: "Cookies and Web Beacons" }),
                DefaultService.createElement("p", {}, [], {
                    innerHTML: 'Like any other website, Naveo uses cookies. These cookies are used to store information including visitors preferences, and the pages on the website that the visitor accessed or visited.The information is used to optimize the users experience by customizing our web page content based on visitors browser type and/ or other information.For more general information on cookies, please read "What Are Cookies".'

                }),


                DefaultService.createElement("p", { class: "f-weight-bold" }, [], { innerHTML: "Advertising Partners Privacy Policies" }),
                DefaultService.createElement("p", {}, [], {
                    innerHTML: 'You may consult this list to find the Privacy Policy for each of the advertising partners of Naveo.'
                        + 'Third-party ad servers or ad networks uses technologies like cookies, JavaScript, or Web Beacons that are used in their respective advertisements and links that appear on Naveo, which are sent directly to users browser.They automatically receive your IP address when this occurs.These technologies are used to measure the effectiveness of their advertising campaigns and/ or to personalize the advertising content that you see on websites that you visit. '
                        + 'Note that Naveo has no access to or control over these cookies that are used by third-party advertisers.'

                }),

                DefaultService.createElement("p", { class: "f-weight-bold" }, [], { innerHTML: "Third Party Privacy Policies" }),
                DefaultService.createElement("p", {}, [], {
                    innerHTML: 'Naveo`s Privacy Policy does not apply to other advertisers or websites.Thus, we are advising you to consult the respective Privacy Policies of these third- party ad servers for more detailed information.It may include their practices and instructions about how to opt - out of certain options.'
                        + 'You can choose to disable cookies through your individual browser options. To know more detailed information about cookie management with specific web browsers, it can be found at the browsers respective websites. '

                }),

                DefaultService.createElement("p", { class: "f-weight-bold" }, [], { innerHTML: "CCPA Privacy Rights (Do Not Sell My Personal Information)" }),
                DefaultService.createElement("p", {}, [], {
                    innerHTML: 'Under the CCPA, among other rights, California consumers have the right to:.'
                        + 'Request that a business that collects a consumer`s personal data disclose the categories and specific pieces of personal data that a business has collected about consumers. '
                        + 'Request that a business delete any personal data about the consumer that a business has collected.'
                        + 'Request that a business that sells a consumer`s personal data, not sell the consumer`s personal data.'
                        + 'If you make a request, we have one month to respond to you. If you would like to exercise any of these rights, please contact us.'

                }),

                DefaultService.createElement("p", { class: "f-weight-bold" }, [], { innerHTML: "GDPR Data Protection Rights" }),
                DefaultService.createElement("p", {}, [], {
                    innerHTML: 'We would like to make sure you are fully aware of all of your data protection rights. Every user is entitled to the following: '
                        + 'The right to access – You have the right to request copies of your personal data. We may charge you a small fee for this service.'
                        + 'The right to rectification – You have the right to request that we correct any information you believe is inaccurate. You also have the right to request that we complete the information you believe is incomplete.'
                        + 'The right to erasure – You have the right to request that we erase your personal data, under certain conditions.'
                        + 'The right to restrict processing – You have the right to request that we restrict the processing of your personal data, under certain conditions.'
                        + 'The right to object to processing – You have the right to object to our processing of your personal data, under certain conditions.'
                        + 'The right to data portability – You have the right to request that we transfer the data that we have collected to another organization, or directly to you, under certain conditions.'
                        + 'If you make a request, we have one month to respond to you. If you would like to exercise any of these rights, please contact us.'

                }),

                DefaultService.createElement("p", { class: "f-weight-bold" }, [], { innerHTML: "Children's Information" }),
                DefaultService.createElement("p", {}, [], {
                    innerHTML: 'Another part of our priority is adding protection for children while using the internet. We encourage parents and guardians to observe, participate in, and/or monitor and guide their online activity.'
                        + 'Naveo does not knowingly collect any Personal Identifiable Information from children under the age of 13. If you think that your child provided this kind of information on our website, we strongly encourage you to contact us immediately and we will do our best efforts to promptly remove such information from our records. '

                }),
            ])

        ]);
        $(".contentCompanyInfo").html(policyContent);

    }

    //Terms and Conditions
    termsConditions() {
        $(".contentCompanyInfo").html("");
        let Termscontent = DefaultService.createElement("div", {}, [
            //DefaultService.createElement("img", { class: "popupTopIcon" }, [], { id: "headerLogo", src: "/asset/image/Common/Logo/ebmslogo.png" }),
            DefaultService.createElement("div", { class: "popupContent m-4" }, [

                DefaultService.createElement("h3", { class: "f-weight-bold" }, [], { innerHTML: "Terms and Conditions" }),

                DefaultService.createElement("p", { class: "f-weight-bold" }, [], {
                    innerHTML: "Please read these terms and conditions carefully before using Our Service."
                }),

                DefaultService.createElement("p", { class: "f-weight-bold" }, [], { innerHTML: "Interpretation and Definitions" }),


                DefaultService.createElement("p", { class: "f-weight-bold" }, [], { innerHTML: "Interpretation" }),
                DefaultService.createElement("p", {}, [], {
                    innerHTML: 'The words of which the initial letter is capitalized have meanings defined under the following conditions.'
                        + 'The following definitions shall have the same meaning regardless of whether they appear in singular or in plural.'

                }),

                DefaultService.createElement("p", { class: "f-weight-bold" }, [], { innerHTML: "Definitions" }),
                DefaultService.createElement("p", {}, [], {
                    innerHTML: 'For the purposes of these Terms and Conditions:'
                        + '   (a) Affiliate means an entity that controls, is controlled by or is under common control with a party, where "control" means ownership of 50% or more of the shares, equity interest or other securities entitled to vote for election of directors or other managing authority.'
                        + ' (b) Company (referred to as either "the Company", "We", "Us" or "Our" in this Agreement) refers to Naveo, Cassis.'
                        + ' (c) Country refers to: Mauritius'
                        + ' (d) Device means any device that can access the Service such as a computer, a cellphone or a digital tablet.'
                        + ' (e) Service refers to the Website.'
                        + ' (f) Terms and Conditions (also referred as "Terms") mean these Terms and Conditions that form the entire agreement between You and the Company regarding the use of the Service. This Terms and Conditions agreement has been created with the help of the Terms and Conditions Generator.'
                        + ' (g) Third-party Social Media Service means any services or content (including data, information, products or services) provided by a third-party that may be displayed, included or made available by the Service.'
                        + ' (h) Website refers to Naveo, accessible from https://www.naveo.mu'
                        + ' (h) You means the individual accessing or using the Service, or the company, or other legal entity on behalf of which such individual is accessing or using the Service, as applicable.'
                }),

                DefaultService.createElement("p", { class: "f-weight-bold" }, [], { innerHTML: "Links to Other Websites" }),
                DefaultService.createElement("p", {}, [], {
                    innerHTML: 'Our Service may contain links to third-party web sites or services that are not owned or controlled by the Company.'
                        + 'The Company has no control over, and assumes no responsibility for, the content, privacy policies, or practices of any third party web sites or services. You further acknowledge and agree that the Company shall not be responsible or liable, directly or indirectly, for any damage or loss caused or alleged to be caused by or in connection with the use of or reliance on any such content, goods or services available on or through any such web sites or services.'
                        + 'We strongly advise You to read the terms and conditions and privacy policies of any third-party web sites or services that You visit.'

                }),
                DefaultService.createElement("p", { class: "f-weight-bold" }, [], { innerHTML: "Termination" }),
                DefaultService.createElement("p", {}, [], {
                    innerHTML: 'We may terminate or suspend Your access immediately, without prior notice or liability, for any reason whatsoever, including without limitation if You breach these Terms and Conditions.'
                        + 'Upon termination, Your right to use the Service will cease immediately.'

                }),

                DefaultService.createElement("p", { class: "f-weight-bold" }, [], { innerHTML: "Limitation of Liability" }),
                DefaultService.createElement("p", {}, [], {
                    innerHTML: 'Notwithstanding any damages that You might incur, the entire liability of the Company and any of its suppliers under any provision of this Terms and Your exclusive remedy for all of the foregoing shall be limited to the amount actually paid by You through the Service or 100 USD if You haven`t purchased anything through the Service. '
                        + 'To the maximum extent permitted by applicable law, in no event shall the Company or its suppliers be liable for any special, incidental, indirect, or consequential damages whatsoever (including, but not limited to, damages for loss of profits, loss of data or other information, for business interruption, for personal injury, loss of privacy arising out of or in any way related to the use of or inability to use the Service, third-party software and/or third-party hardware used with the Service, or otherwise in connection with any provision of this Terms), even if the Company or any supplier has been advised of the possibility of such damages and even if the remedy fails of its essential purpose.'
                        + 'Some states do not allow the exclusion of implied warranties or limitation of liability for incidental or consequential damages, which means that some of the above limitations may not apply. In these states, each party`s liability will be limited to the greatest extent permitted by law.'

                }),

                DefaultService.createElement("p", { class: "f-weight-bold" }, [], { innerHTML: "'AS IS' and 'AS AVAILABLE' Disclaimer" }),
                DefaultService.createElement("p", {}, [], {
                    innerHTML: 'The Service is provided to You "AS IS" and "AS AVAILABLE" and with all faults and defects without warranty of any kind. To the maximum extent permitted under applicable law, the Company, on its own behalf and on behalf of its Affiliates and its and their respective licensors and service providers, expressly disclaims all warranties, whether express, implied, statutory or otherwise, with respect to the Service, including all implied warranties of merchantability, fitness for a particular purpose, title and non-infringement, and warranties that may arise out of course of dealing, course of performance, usage or trade practice. Without limitation to the foregoing, the Company provides no warranty or undertaking, and makes no representation of any kind that the Service will meet Your requirements, achieve any intended results, be compatible or work with any other software, applications, systems or services, operate without interruption, meet any performance or reliability standards or be error free or that any errors or defects can or will be corrected.'
                        + 'Without limiting the foregoing, neither the Company nor any of the company`s provider makes any representation or warranty of any kind, express or implied: (i) as to the operation or availability of the Service, or the information, content, and materials or products included thereon; (ii) that the Service will be uninterrupted or error - free; (iii) as to the accuracy, reliability, or currency of any information or content provided through the Service; or(iv) that the Service, its servers, the content, or e - mails sent from or on behalf of the Company are free of viruses, scripts, trojan horses, worms, malware, timebombs or other harmful components. '
                        + 'Some jurisdictions do not allow the exclusion of certain types of warranties or limitations on applicable statutory rights of a consumer, so some or all of the above exclusions and limitations may not apply to You. But in such a case the exclusions and limitations set forth in this section shall be applied to the greatest extent enforceable under applicable law.'

                }),

                DefaultService.createElement("p", { class: "f-weight-bold" }, [], { innerHTML: "Governing Law" }),
                DefaultService.createElement("p", {}, [], {
                    innerHTML: 'The laws of the Country, excluding its conflicts of law rules, shall govern this Terms and Your use of the Service. Your use of the Application may also be subject to other local, state, national, or international laws. '

                }),

                DefaultService.createElement("p", { class: "f-weight-bold" }, [], { innerHTML: "Disputes Resolution" }),
                DefaultService.createElement("p", {}, [], {
                    innerHTML: 'If You have any concern or dispute about the Service, You agree to first try to resolve the dispute informally by contacting the Company.'

                }),


                DefaultService.createElement("p", { class: "f-weight-bold" }, [], { innerHTML: "For European Union (EU) Users" }),
                DefaultService.createElement("p", {}, [], {
                    innerHTML: 'If You are a European Union consumer, you will benefit from any mandatory provisions of the law of the country in which you are resident in.'

                }),


                DefaultService.createElement("p", { class: "f-weight-bold" }, [], { innerHTML: "Severability and Waiver" }),

                DefaultService.createElement("p", { class: "f-weight-bold" }, [], { innerHTML: "Severability" }),
                DefaultService.createElement("p", {}, [], {
                    innerHTML: 'If any provision of these Terms is held to be unenforceable or invalid, such provision will be changed and interpreted to accomplish the objectives of such provision to the greatest extent possible under applicable law and the remaining provisions will continue in full force and effect.'

                }),

                DefaultService.createElement("p", { class: "f-weight-bold" }, [], { innerHTML: "Waiver" }),
                DefaultService.createElement("p", {}, [], {
                    innerHTML: 'Except as provided herein, the failure to exercise a right or to require performance of an obligation under this Terms shall not effect a party`s ability to exercise such right or require such performance at any time thereafter nor shall be the waiver of a breach constitute a waiver of any subsequent breach.'

                }),

                DefaultService.createElement("p", { class: "f-weight-bold" }, [], { innerHTML: "Translation Interpretation" }),
                DefaultService.createElement("p", {}, [], {
                    innerHTML: 'These Terms and Conditions may have been translated if We have made them available to You on our Service. You agree that the original English text shall prevail in the case of a dispute.'

                }),

                DefaultService.createElement("p", { class: "f-weight-bold" }, [], { innerHTML: "Changes to These Terms and Conditions" }),
                DefaultService.createElement("p", {}, [], {
                    innerHTML: 'We reserve the right, at Our sole discretion, to modify or replace these Terms at any time. If a revision is material We will make reasonable efforts to provide at least 30 days` notice prior to any new terms taking effect.What constitutes a material change will be determined at Our sole discretion.'
                        + 'By continuing to access or use Our Service after those revisions become effective, You agree to be bound by the revised terms. If You do not agree to the new terms, in whole or in part, please stop using the website and the Service.'
                }),
            ])

        ]);

        $(".contentCompanyInfo").html(Termscontent);
    }


    //the about popup
    aboutNaveo() {
        $(".contentCompanyInfo").html("");
        let AboutUscontent = DefaultService.createElement("div", {}, [

            DefaultService.createElement("div", { class: "popupContent m-4" }, [
                DefaultService.createElement("h3", { class: "f-weight-bold" }, [], { innerHTML: "About Us" }),
                DefaultService.createElement("p", { class: "f-weight-bold" }, [], { innerHTML: "LEADER IN THE AFRICAN MARKET" }),
                DefaultService.createElement("p", {}, [], {
                    innerHTML: 'Naveo isthe most innovative and complete  platform for messaging and performance in the real time. The naveo platform can be customised to adapt to your needs and metrics to <br>Optimise Costs <br>Improve Performance <br>Ensure Safety and Compliance'
                }),
                DefaultService.createElement("p", { class: "f-weight-bold" }, [], { innerHTML: "THE NAVEO LATFORM" }),
                DefaultService.createElement("p", {}, [], {
                    innerHTML: 'The Naveo Platform is an all-in-one platform that encompasses a Telematics Solution, an Internet of Things (IoT) platform and a Geographical Information System (GIS)that helps you to radically transform your business performance by giving you access to cutting-edge information on your out of sight activities'
                }),
            ])

        ]);

        $(".contentCompanyInfo").html(AboutUscontent);

    }
}

window.policyAndTerms = new policyAndTerms();