import { Button } from "@/components/ui/button";
import {
    Table,
    TableBody,
    TableCaption,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from "@/components/ui/table"
import Link from "next/link";
import { Badge } from "@/components/ui/badge";
import { cookies } from "next/headers";

const BACKEND_URL = process.env.BACKEND_URL || "http://localhost:5226";

function TH({ children }) {
    return (
        <TableHead className={"text-center"}>
            {children}
        </TableHead>
    )
}

function Cell({ children }) {
    return (
        <TableCell className={"text-center"}>
            {children}
        </TableCell>
    )
}


export default async function Clients(props) {
    const cookieStore = await cookies();
    const session = cookieStore.get("session")?.value;

    let error;


    let Clients = [];
    try {
        const res = await fetch(`${process.env.NEXTJS_URL}/api/clients`, {
            headers: {
                Cookie: `session=${session}`,
            },
            cache: "no-store",
        });
        // if (!res.ok) throw new Error('Failed to load Clients');
        if (res.ok)
            Clients = await res.json();
        else
            error = "Failed to load clients"
    } catch (e) {
        // console.error("Failed to load Clients,", e);
        Clients = [];
    }
    return (
        <main className="flex flex-col h-min-full container mx-auto p-4">
            <div className="container mx-auto my-12">
                <h1 className="text-5xl font-bold mb-8">Clients</h1>
            </div>
            <div className="container mx-auto m-4">
                <Link className="bg-identity object flex p-2 text-background rounded-sm w-fit" href={"clients/new"}>Add New Client</Link>
            </div>

            {
                !error ?
                    (<>
                        {
                            Clients.length > 0 || <div className="container mx-auto">No clients has been made</div>
                        }
                        <Table className={"container mx-auto border-2 border-identity-dillute/20 rounded-xl "}>
                            <TableHeader >
                                <TableRow className={"bg-accent rounded-xl"}>
                                    <TH>Customer</TH>
                                    <TH>Contact Number</TH>
                                    <TH>Company Name</TH>
                                    <TH>Company Address</TH>
                                    <TH>Email</TH>
                                </TableRow>
                            </TableHeader>
                            <TableBody>
                                {
                                    Clients.map((Client, index) => (
                                        <TableRow key={index} >
                                            <Cell>
                                                <h1 className="font-bold">{Client.clientName}</h1>
                                                <p className="">{Client.clientId}</p>
                                            </Cell>
                                            <Cell>
                                                010tekan2xdpt
                                            </Cell>
                                            <Cell>
                                                {Client.companyName}
                                            </Cell>
                                            <Cell>
                                                {Client.CompanyAddress}
                                            </Cell>
                                            <Cell>
                                                {Client.email}
                                            </Cell>
                                        </TableRow>
                                    ))
                                }


                            </TableBody>

                        </Table>
                    </>)
                    :
                    (<>
                        <Badge variant={"destructive"} className={"mx-auto"}>{error}</Badge>
                        <Table className={"container mx-auto border-2 border-identity-dillute/20 rounded-xl "}>
                            <TableHeader >
                                <TableRow className={"bg-accent rounded-xl"}>
                                    <TH>Customers</TH>
                                    <TH>Status</TH>
                                    <TH>Order Date

                                    </TH>
                                    <TH>Due Date</TH>
                                    <TH>Generate Client</TH>
                                    <TH>Send an email</TH>
                                    <TH>Receipt</TH>
                                </TableRow>
                            </TableHeader>
                            <TableBody></TableBody>
                        </Table>
                    </>)
            }

        </main>
    )
}

const dummyCustomers = [
    {
        "": 2,
        "": 4,
        "": "bruh",
        "": "bruh",
        "": "bruh@gmail.com"
    },];