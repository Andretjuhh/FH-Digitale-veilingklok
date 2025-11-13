// External imports
import React from 'react';
import clsx from "clsx";

// Internal imports
import {joinClsx} from "../../utils/classPrefixer";


type FormLinkProps = {
    labelClassName?: string;
    label: string;  
} & React.ButtonHTMLAttributes<HTMLButtonElement>; 

function FormLink(props: FormLinkProps) {
    const { label, className, labelClassName, ...rest } = props; 
    
    return <button 
        className={clsx('base-btn-link', className)} // These classes likely make it look like a link
        type="button" // Good practice to explicitly set type for non-submit buttons
        {...rest}
    >        
        {label && <span className={clsx('base-btn-link-txt', joinClsx(className, 'txt'), labelClassName)}>{label}</span>}
        
    </button>;
}

export default FormLink;