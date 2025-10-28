import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import "../styles/register.css";

type UserType = "koper" | "kweker" | "veilingmeester";

interface InputField {
  label: string;
  type: string;
  placeholder?: string;
  options?: string[];
}

function Register() {
  const navigate = useNavigate();
  const [userType, setUserType] = useState<UserType>("koper");

  // Define input fields for each user type
  const fields: Record<UserType, InputField[]> = {
  koper: [
    { label: "Email", type: "email", placeholder: "you@example.com" },
    { label: "Password", type: "password", placeholder: "••••••••" },
    { label: "First Name", type: "text", placeholder: "Steve" },
    { label: "Last Name", type: "text", placeholder: "Jobs" },
    { label: "Address", type: "text", placeholder: "Street 123" },
    { label: "Postcode", type: "text", placeholder: "1234 AB" },
    { label: "Region", type: "select", options: ["Noord-Holland","Zuid-Holland","Utrecht","Gelderland","Overijssel","Limburg","Friesland","Drenthe","Flevoland","Groningen","Zeeland"] },
  ],
  kweker: [
    { label: "Email", type: "email", placeholder: "you@example.com" },
    { label: "Password", type: "password", placeholder: "••••••••" },
    { label: "Company Name", type: "text", placeholder: "Example BV" },
    { label: "Telphone Number", type: "text", placeholder: "+31 6 12345678"},
    { label: "Address", type: "text", placeholder: "Street 123" },
    { label: "Postcode", type: "text", placeholder: "1234 AB" },
    { label: "Region", type: "select", options: ["Noord-Holland","Zuid-Holland","Utrecht","Gelderland","Overijssel","Limburg","Friesland","Drenthe","Flevoland","Groningen","Zeeland"] },
    { label: "kvk Number", type: "text", placeholder: "12345678"},
  ],
  veilingmeester: [
    { label: "Email", type: "email", placeholder: "you@example.com" },
    { label: "Password", type: "password", placeholder: "••••••••" },
    { label: "Region", type: "select", options: ["Noord-Holland","Zuid-Holland","Utrecht","Gelderland","Overijssel","Limburg","Friesland","Drenthe","Flevoland","Groningen","Zeeland"] },
    { label: "Authorisation Code", type: "text", placeholder: "123456" },  
  ],
};


  return (
    <div className="register-page">
      <div className="register-card">
        <div className="register-header">
          <h2 className="register-title">Create Account</h2>
          <p className="register-subtitle">Please fill in the information below</p>
        </div>

        {/* Tabs */}
        <div className="register-tabs">
          {(["koper", "kweker", "veilingmeester"] as UserType[]).map((type) => (
            <div
                key={type}
                className={`register-tab ${userType === type ? "active" : ""}`}
                onClick={() => setUserType(type)}
            >
                {type.charAt(0).toUpperCase() + type.slice(1)}
            </div>
            ))}

        </div>

        {/* Form */}
        <form className="register-form">
          {fields[userType].map((field, idx) => {
            if (field.type === "select") {
              return (
                <div key={idx}>
                  <label>{field.label}</label>
                  <select className="input-field" defaultValue="">
                    <option value="" disabled>
                      Select {field.label.toLowerCase()}
                    </option>
                    {field.options!.map((option) => (
                      <option key={option} value={option}>
                        {option}
                      </option>
                    ))}
                  </select>
                </div>
              );
            }

            return (
              <div key={idx}>
                <label>{field.label}</label>
                <input
                  type={field.type}
                  placeholder={field.placeholder}
                  className="input-field"
                />
              </div>
            );
          })}

          <button type="submit" className="btn-primary">
            Create Account
          </button>

          <button
            type="button"
            className="login-link"
            onClick={() => navigate("/login")}
          >
            Login instead?
          </button>
        </form>
      </div>
    </div>
  );
}

export default Register;
