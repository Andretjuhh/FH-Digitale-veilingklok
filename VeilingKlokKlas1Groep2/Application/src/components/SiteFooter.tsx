import React from "react";
import { Link } from "react-router-dom";
import "../styles/SiteFooter.css";

export default function SiteFooter() {
  return (
    <footer className="rfh-footer" role="contentinfo">
      <div className="rfh-footer__top">
        <div className="rfh-footer__brand">
          <div className="rfh-footer__logo" aria-hidden>VK</div>
          <div className="rfh-footer__title">Veilingplatform</div>
        </div>

        <nav className="rfh-footer__links" aria-label="Footer links">
          <ul>
            <li><Link to="/status">Storingen / onderhoud</Link></li>
            <li><Link to="/apps">Onze apps</Link></li>
            <li><Link to="/pers">Pers & media</Link></li>
            <li><Link to="/nieuwsbrief">Aanmelden nieuwsbrief</Link></li>
            <li><Link to="/intranet">Intranet</Link></li>
          </ul>
          <ul>
            <li><Link to="/service">Service & contact</Link></li>
            <li><Link to="/remote-help">Hulp op afstand</Link></li>
            <li><a href="https://wa.me/31600000000" target="_blank" rel="noreferrer">WhatsApp</a></li>
            <li><Link to="/contact">Contactformulier</Link></li>
            <li><Link to="/locaties">Locaties</Link></li>
          </ul>
        </nav>

        <div className="rfh-footer__social" aria-label="Social media">
          <a aria-label="LinkedIn" href="#" className="soc"><LinkedInIcon/></a>
          <a aria-label="YouTube"  href="#" className="soc"><YouTubeIcon/></a>
          <a aria-label="Facebook" href="#" className="soc"><FacebookIcon/></a>
          <a aria-label="Instagram" href="#" className="soc"><InstagramIcon/></a>
        </div>

        {/* decoratieve curve */}
        <div className="rfh-footer__curve" aria-hidden />
      </div>

      <div className="rfh-footer__bottom">
        <div>© {new Date().getFullYear()} Veilingplatform</div>
        <nav aria-label="Juridisch">
          <a href="#">Privacyverklaring</a>
          <a href="#">Cookieverklaring</a>
          <a href="#">CVD</a>
          <a href="#">Algemene voorwaarden</a>
        </nav>
      </div>
    </footer>
  );
}

/* --- simpele inline iconen --- */
function LinkedInIcon(){return(
  <svg viewBox="0 0 24 24" width="18" height="18" aria-hidden><path fill="currentColor" d="M4.98 3.5C4.98 4.88 3.86 6 2.5 6S0 4.88 0 3.5 1.12 1 2.5 1s2.48 1.12 2.48 2.5zM.5 8h4V23h-4V8zM8 8h3.8v2.05h.06c.53-1 1.83-2.05 3.77-2.05 4.03 0 4.77 2.65 4.77 6.1V23h-4v-7.1c0-1.7-.03-3.9-2.38-3.9-2.38 0-2.75 1.85-2.75 3.78V23h-4V8z"/></svg>
);}
function YouTubeIcon(){return(
  <svg viewBox="0 0 24 24" width="18" height="18" aria-hidden><path fill="currentColor" d="M23.5 6.2a3 3 0 00-2.1-2.1C19.3 3.5 12 3.5 12 3.5s-7.3 0-9.4.6A3 3 0 00.5 6.2 31 31 0 000 12a31 31 0 00.5 5.8 3 3 0 002.1 2.1c2.1.6 9.4.6 9.4.6s7.3 0 9.4-.6a3 3 0 002.1-2.1A31 31 0 0024 12a31 31 0 00-.5-5.8zM9.75 15.5v-7L16 12l-6.25 3.5z"/></svg>
);}
function FacebookIcon(){return(
  <svg viewBox="0 0 24 24" width="18" height="18" aria-hidden><path fill="currentColor" d="M22 12a10 10 0 10-11.6 9.9v-7h-2.3V12h2.3V9.8c0-2.3 1.4-3.6 3.5-3.6 1 0 2 .2 2 .2v2.2h-1.1c-1.1 0-1.5.7-1.5 1.4V12h2.6l-.4 2.9h-2.2v7A10 10 0 0022 12z"/></svg>
);}
function InstagramIcon(){return(
  <svg viewBox="0 0 24 24" width="18" height="18" aria-hidden><path fill="currentColor" d="M12 2.2c3.2 0 3.6 0 4.9.1 1.2.1 1.9.2 2.4.4.6.2 1 .5 1.4.9.4.4.7.8.9 1.4.2.5.3 1.2.4 2.4.1 1.3.1 1.7.1 4.9s0 3.6-.1 4.9c-.1 1.2-.2 1.9-.4 2.4a3.6 3.6 0 01-.9 1.4 3.6 3.6 0 01-1.4.9c-.5.2-1.2.3-2.4.4-1.3.1-1.7.1-4.9.1s-3.6 0-4.9-.1c-1.2-.1-1.9-.2-2.4-.4a3.6 3.6 0 01-1.4-.9 3.6 3.6 0 01-.9-1.4c-.2-.5-.3-1.2-.4-2.4C2.2 15.6 2.2 15.2 2.2 12s0-3.6.1-4.9c.1-1.2.2-1.9.4-2.4.2-.6.5-1 .9-1.4.4-.4.8-.7 1.4-.9.5-.2 1.2-.3 2.4-.4C8.4 2.2 8.8 2.2 12 2.2zm0 1.8c-3.1 0-3.5 0-4.7.1-1 .1-1.6.2-2 .4-.5.2-.8.4-1.1.7-.3.3-.5.6-.7 1.1-.2.4-.3 1-.4 2-.1 1.2-.1 1.6-.1 4.7s0 3.5.1 4.7c.1 1 .2 1.6.4 2 .2.5.4.8.7 1.1.3.3.6.5 1.1.7.4.2 1 .3 2 .4 1.2.1 1.6.1 4.7.1s3.5 0 4.7-.1c1-.1 1.6-.2 2-.4.5-.2.8-.4 1.1-.7.3-.3.5-.6.7-1.1.2-.4.3-1 .4-2 .1-1.2.1-1.6.1-4.7s0-3.5-.1-4.7c-.1-1-.2-1.6-.4-2-.2-.5-.4-.8-.7-1.1-.3-.3-.6-.5-1.1-.7-.4-.2-1-.3-2-.4-1.2-.1-1.6-.1-4.7-.1zm0 2.9a5.2 5.2 0 110 10.4 5.2 5.2 0 010-10.4zm0 1.8a3.4 3.4 0 100 6.8 3.4 3.4 0 000-6.8zm5.6-2.2a1.2 1.2 0 110 2.4 1.2 1.2 0 010-2.4z"/></svg>
);}
