"use client"

import * as React from "react"
import { CheckIcon, ChevronsUpDownIcon } from "lucide-react"
import { cn } from "@/lib/utils"
import { Button } from "@/components/ui/button"
import {
    Command,
    CommandEmpty,
    CommandGroup,
    CommandInput,
    CommandItem,
    CommandList,
} from "@/components/ui/command"
import {
    Popover,
    PopoverContent,
    PopoverTrigger,
} from "@/components/ui/popover"

export function ClientSelectCombobox({ value, onChange }) {
    const [open, setOpen] = React.useState(false)
    const [clients, setClients] = React.useState([])
    const [loading, setLoading] = React.useState(true)

    React.useEffect(() => {
        async function fetchClients() {
            setLoading(true)
            try {
                const res = await fetch("/api/clients", { cache: "no-store" })
                if (res.ok) {
                    const data = await res.json()
                    setClients(data)
                } else {
                    setClients([])
                }
            } catch (e) {
                setClients([])
            }
            setLoading(false)
        }
        fetchClients()
    }, [])

    return (
        <Popover open={open} onOpenChange={setOpen}>
            <PopoverTrigger asChild>
                <Button
                    variant="outline"
                    role="combobox"
                    aria-expanded={open}
                    className="w-[200px] justify-between"
                >
                    {value
                        ? clients.find((c) => c.clientId === value)?.clientName || value
                        : loading ? "Loading..." : "Select client..."}
                    <ChevronsUpDownIcon className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                </Button>
            </PopoverTrigger>
            <PopoverContent className="w-[200px] p-0">
                <Command>
                    <CommandInput placeholder="Search client..." />
                    <CommandList>
                        <CommandEmpty>No client found.</CommandEmpty>
                        <CommandGroup>
                            {clients.map((client) => (
                                <CommandItem
                                    key={client.clientId}
                                    value={client.clientId}
                                    onSelect={() => {
                                        onChange(client.clientId)
                                        setOpen(false)
                                    }}
                                >
                                    <CheckIcon
                                        className={cn(
                                            "mr-2 h-4 w-4",
                                            value === client.clientId ? "opacity-100" : "opacity-0"
                                        )}
                                    />
                                    {client.clientName}
                                </CommandItem>
                            ))}
                        </CommandGroup>
                    </CommandList>
                </Command>
            </PopoverContent>
        </Popover>
    )
}
