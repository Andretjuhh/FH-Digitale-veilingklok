import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import Button from "../../buttons/Button";
import FormInputField from "../../elements/FormInputField";

type UserType = "koper" | "kweker" | "veilingmeester";

interface InputField {
  label: string;
  type: string;
  placeholder?: string;
  options?: string[];
}

function RegisterContent() {
  const navigate = useNavigate();
  const [userType, setUserType] = useState<UserType>("koper");
  const [step, setStep] = useState(1);
  const [formData, setFormData] = useState<Record<string, string>>({});
  const handleGoBack = () => navigate("/");

  const regions = [
    "Noord-Holland",
    "Zuid-Holland",
    "Utrecht",
    "Gelderland",
    "Overijssel",
    "Limburg",
    "Friesland",
    "Drenthe",
    "Flevoland",
    "Groningen",
    "Zeeland",
  ];

  const t = window.application.t;

  const steps: Record<UserType, InputField[][]> = {
    koper: [
      [
        { label: t("email"), type: "email", placeholder: "you@example.com" },
        { label: t("password"), type: "password", placeholder: "••••••••" },
        { label: t("first_name"), type: "text", placeholder: "Steve" },
        { label: t("last_name"), type: "text", placeholder: "Jobs" },
      ],
      [
        { label: t("address"), type: "text", placeholder: "Street 123" },
        { label: t("postcode"), type: "text", placeholder: "1234 AB" },
        { label: t("region"), type: "select", options: regions },
      ],
    ],
    kweker: [
      [
        { label: t("company_name"), type: "text", placeholder: "Example BV" },
        { label: t("email"), type: "email", placeholder: "you@example.com" },
        { label: t("password"), type: "password", placeholder: "••••••••" },
        { label: t("kvk_number"), type: "text", placeholder: "12345678" },
      ],
      [
        { label: t("phonenumber"), type: "text", placeholder: "+31 6 12345678" },
        { label: t("address"), type: "text", placeholder: "Street 123" },
        { label: t("postcode"), type: "text", placeholder: "1234 AB" },
        { label: t("region"), type: "select", options: regions },
      ],
    ],
    veilingmeester: [
      [
        { label: t("email"), type: "email", placeholder: "you@example.com" },
        { label: t("password"), type: "password", placeholder: "••••••••" },
      ],
      [
        { label: t("region"), type: "select", options: regions },
        { label: t("authorisation_code"), type: "text", placeholder: "123456" },
      ],
    ],
  };

  const totalSteps = steps[userType].length;
  const currentFields = steps[userType][step - 1];

  const handleInputChange = (key: string, value: string) => {
    setFormData((prev) => ({ ...prev, [key]: value }));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    console.log("Final submitted data:", formData);

    let destination = "/";
    switch (userType) {
      case "koper":
        destination = "/user-dashboard";
        break;
      case "kweker":
        destination = "/kweker";
        break;
      case "veilingmeester":
        destination = "/dashboard";
        break;
    }

    alert(`Account created successfully as ${userType}! Redirecting...`);
    navigate(destination);
  };

  return (
    <div className="app-page register-page">
      <div className="register-card">
        <div className="register-header">
          <Button
          className="logo-icon"
          icon="/svg/logo-flori.svg"
          onClick={handleGoBack}
          aria-label={t("back_button_aria")}
        />
          <div className="register-text-container">
            <h2 className="register-title" aria-label={t('create_account')}>{t("create_account")}</h2>
            <p className="register-subtitle" aria-live="polite">
              {t("step")} {step} {t("of")} {totalSteps}
            </p>
          </div> 
        </div>
        <div className="registration-stepper" aria-label={`Registration step ${step} of ${totalSteps}`}>
          {/* Map over the total number of steps (1 and 2) */}
          {Array.from({ length: totalSteps }, (_, index) => {
            const stepNumber = index + 1;
            const isActive = stepNumber === step;
            const isCompleted = stepNumber < step;
            
            return (
              <React.Fragment key={stepNumber}>
                <div 
                  className={`stepper-dot ${isActive ? 'active' : ''} ${isCompleted ? 'completed' : ''}`}
                  role="presentation"
                >
                  {/* Optional: Add a checkmark for completed steps */}
                  {isCompleted ? <span className="checkmark">✓</span> : stepNumber}
                </div>
                
                {/* Connector line between dots */}
                {stepNumber < totalSteps && (
                  <div className={`stepper-connector ${isCompleted ? 'completed' : ''}`} />
                )}
              </React.Fragment>
            );
          })}
        </div>

        <div className="register-tabs" role="tablist" aria-label="Select user type">
          {(["koper", "kweker", "veilingmeester"] as UserType[]).map((type) => (
            <div
              key={type}
              className={`register-tab ${userType === type ? "active" : ""}`}
              onClick={() => {
                setUserType(type);
                setStep(1);
              }}
              role="tab"
              aria-selected={userType === type}
              tabIndex={0}
              aria-label={`Register as ${type}`}
            >
              {type.charAt(0).toUpperCase() + type.slice(1)}
            </div>
          ))}
        </div>

        <form className="register-form" onSubmit={handleSubmit}>
          {currentFields.map((field, idx) => {
            const key = `${userType}-${field.label}`;
            const ariaLabel = `${field.label} for ${userType}`;

            return (
              <FormInputField
                key={idx}
                id={key}
                label={field.label}
                placeholder={field.placeholder}
                type={field.type === "select" ? "select" : field.type}
                value={formData[key] || ""}
                onChange={(e) =>
                  handleInputChange(
                    key,
                    field.type === "select"
                      ? e.target.value
                      : (e.target as HTMLInputElement).value
                  )
                }
                className="input-field"
                isError={false}
                error=""
                aria-label={ariaLabel}
              />
            );
          })}

          <div className="form-buttons">
            {step > 1 && (
              <Button className='btn-secondary' label={window.application.t('previous')} onClick={() => setStep(step - 1)} aria-label={window.application.t('previous')} type="button"/>
            )}

            {step < totalSteps ? (
              <Button className='btn-secondary' label={window.application.t('next')} onClick={() => setStep(step + 1)} aria-label={window.application.t('next')}/>
            ) : (
              <Button className='btn-primary' label= {window.application.t('create_account')} aria-label={window.application.t('create_account')} type="submit"/>
            )}
          </div>
          <Button className="login-link" label = {window.application.t('login_message')} onClick={() => navigate("/login")} aria-label={window.application.t('login_message_aria')} type="button"/>
        </form>
      </div>
    </div>
  );
}

export default RegisterContent;
